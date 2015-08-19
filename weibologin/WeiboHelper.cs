using MSScriptControl;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace weibo_hot
{
    class WeiboHelper
    {
        public string ReturnCookieString(string uid, string psw, CookieContainer cc)
        {
            if (SinaLogin(uid, psw, cc) == 0)
                return cc.GetCookieHeader(new Uri("http://weibo.com/"));

            else return null;
        }
        public CookieContainer ReturnCookieCookieContainer(string uid, string psw)
        {
            WeiboEntity weibo = new WeiboEntity();

            CookieContainer cc = new CookieContainer();
            if (SinaLogin(uid, psw, cc) == 0)
            {
                Console.WriteLine("已获取到cookies，准备进行采集");
                return cc;
            }
            else return null;
        }


        private int SinaLogin(string uid, string psw, CookieContainer cc) //返回为0时成功登陆，4047为需要输入验证码
        {
            try
            {
                string uidbase64 = Base64Code(uid); //处理登录账户 如***@**.com 
                uidbase64 = HttpUtility.HtmlEncode(uidbase64);

                string url = "http://login.sina.com.cn/sso/prelogin.php?entry=weibo&callback=sinaSSOController.preloginCallBack&su="
                    + uidbase64 + "&rsakt=mod&checkpin=1" + "&client=ssologin.js(v1.4.15)" + "&_=" + KCIS.App.Utilities.DateTimeHelper.DateTimeToUnixTimeStamp(DateTime.Now); //使用工具类将datetime转换为timestamp

                HttpWebRequest webRequest1 = (HttpWebRequest)WebRequest.Create(new Uri(url)); //获取servertime和 nonce
                webRequest1.CookieContainer = cc;
                webRequest1.Accept = "application/javascript, */*;q=0.8";
                webRequest1.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
                webRequest1.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                webRequest1.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36";
                webRequest1.Referer = "http://weibo.com/";
                webRequest1.Host = "login.sina.com.cn";
                webRequest1.Headers.Add("DNT", "1");


                HttpWebResponse response1 = (HttpWebResponse)webRequest1.GetResponse();
                StreamReader sr1 = new StreamReader(response1.GetResponseStream(), Encoding.UTF8);
                string res = null;
                if (response1.ContentEncoding == "gzip")
                {
                    //StreamReader sr1 = new StreamReader(response1.GetResponseStream(), Encoding.UTF8);
                    //是否要解压缩stream？
                    res = GetResponseByZipStream(response1);
                    Console.WriteLine(res);
                }
                else
                {
                    res = sr1.ReadToEnd();
                }

                //string res = sr1.ReadToEnd();
                response1.Close();
                string servertime = Regex.Match(res, ",\"servertime\":(.*?),").Groups[1].ToString();
                if (servertime == null || servertime == "") return -1;
                string nonce = Regex.Match(res, ",\"nonce\":\"(.*?)\",").Groups[1].ToString();
                if (nonce == null || nonce == "") return -1;
                //pubkey和rsakv
                string pubkey2 = Regex.Match(res, ",\"pubkey\":\"(.*?)\",").Groups[1].ToString();
                string pcid = Regex.Match(res,"\"pcid\":\"(.*?)\",").Groups[1].ToString();
                string rsakv = "1330428213"; //新浪固定
                string Piccode = GetPicCode(cc,pcid);
                string password = hex_sha1("" + hex_sha1(hex_sha1(psw)) + servertime + nonce); //处理新浪微博用户密码psw
                Console.WriteLine("cookie第一步获取完毕");
                string pass = GetPwds(psw, servertime, nonce, pubkey2);
                string str = "entry=weibo&gateway=1&from=&savestate=7&useticket=1&pagerefer=&vsnf=1&su=" +
                    uidbase64 + "&service=miniblog&servertime=" + servertime + "&rsakv=" + rsakv + "&nonce=" + nonce + "&pwencode=" + "rsa2" + "&sp=" + pass + "&encoding=utf-8&url=" +
                    "&prelt=113&url=http%3A%2F%2Fweibo.com%2Fajaxlogin.php%3Fframelogin%3D1%26callback%3Dparent.sinaSSOController.feedBackUrlCallBack&returntype=META";
                if(Piccode.Length>2)
                {
                    str = str + string.Format("&pcid={1}&door={0}", Piccode,pcid);
                }
                byte[] bytes;
                ASCIIEncoding encoding = new ASCIIEncoding();

                bytes = encoding.GetBytes(str);
                HttpWebRequest webRequest2 = (HttpWebRequest)WebRequest.Create("http://login.sina.com.cn/sso/login.php?client=ssologin.js(v1.4.15)");
                webRequest2.Method = "POST";
                webRequest2.ContentType = "application/x-www-form-urlencoded";
                webRequest2.ContentLength = bytes.Length;
                webRequest2.CookieContainer = cc;
                webRequest2.Accept = "application/javascript, */*;q=0.8";
                webRequest2.Headers.Add("Accept-Encoding", "gzip, deflate");
                webRequest2.Headers.Add("Accept-Language", "zh-CN");
                webRequest2.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko";
                webRequest2.Referer = "http://weibo.com/";
                webRequest2.Host = "login.sina.com.cn";
                Stream stream;
                stream = webRequest2.GetRequestStream();
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
                HttpWebResponse response2 = (HttpWebResponse)webRequest2.GetResponse();
                string res2 = null;
                if (response2.ContentEncoding == "gzip")
                {
                    //StreamReader sr1 = new StreamReader(response1.GetResponseStream(), Encoding.UTF8);
                    //是否要解压缩stream？
                    res2 = GetResponseByZipStream(response2);
                    Console.WriteLine(res2);
                }
                else
                {
                    StreamReader sr3 = new StreamReader(response2.GetResponseStream(), Encoding.UTF8);
                    res2 = sr3.ReadToEnd();
                }
                response2.Close();
                //string res2 = GetResponseByZipStream(response2);
                Console.WriteLine("第二步cookie获取");
                //Console.WriteLine(res2);
                Console.WriteLine("第二步cookie获取完毕");
                //StreamReader sr2 = new StreamReader(response2.GetResponseStream(), Encoding.Default);
                //string res2 = sr2.ReadToEnd();
                string error = HttpUtility.UrlDecode(res2, Encoding.GetEncoding("gbk"));
                Console.WriteLine(error);
                int retcode=-1;
                if(Regex.Match(res2, "retcode\":(.*?),").Groups[1].ToString()!="")
                  retcode  = Convert.ToInt32(Regex.Match(res2, "retcode\":(.*?),").Groups[1].ToString());
                if (Regex.Match(res2, "retcode=(.*?)&").Groups[1].ToString() != "")
                    retcode = Convert.ToInt32(Regex.Match(res2, "retcode=(.*?)&").Groups[1].ToString());
                if (retcode != 0) return -1;
                url = Regex.Match(res2, @"location.replace\('(.*?)'").Groups[1].ToString();
                HttpWebRequest webRequest3 = (HttpWebRequest)WebRequest.Create(new Uri(url));
                webRequest3.CookieContainer = cc;
                HttpWebResponse response3 = (HttpWebResponse)webRequest3.GetResponse();
                if (response3.ContentEncoding == "gzip")
                {
                    //StreamReader sr1 = new StreamReader(response1.GetResponseStream(), Encoding.UTF8);
                    //是否要解压缩stream？
                    res = GetResponseByZipStream(response1);
                    //Console.WriteLine(res);
                }
                else
                {
                    StreamReader sr3 = new StreamReader(response3.GetResponseStream(), Encoding.UTF8);
                    res = sr3.ReadToEnd();
                }
                response3.Close();
                //res = WeiboHelper.GetResponseByZipStream(response3);
                //StreamReader sr3 = new StreamReader(response3.GetResponseStream(), Encoding.UTF8);
                //res = sr3.ReadToEnd();
                Console.WriteLine("第3步cookie获取");
                //Console.WriteLine(res);
                Console.WriteLine("第3步cookie获取完毕");
                foreach (Cookie cookie in response3.Cookies)
                {
                    cc.Add(cookie);
                }
                return 0;
            }
            catch(Exception t)
            {
                log4net.ILog log = log4net.LogManager.GetLogger("MyLogger");
                log.Debug(t.Message);
                log.Error("Login出错");
                Console.WriteLine(t.Message);
                Console.WriteLine("Login出错");
                return -1;
                
            }
        }

        private string GetPicCode(CookieContainer cc,string pcid)
        {
            string savePath = @"G:\\test";
            var r = Convert.ToInt32(MathRandom() * 1e8);//1e8 = 100000000;
            var url = string.Format(@"http://login.sina.com.cn/cgi/pin.php?r={0}&s=0&p={1}", r, pcid);
            HttpWebRequest webRequest1 = (HttpWebRequest)WebRequest.Create(new Uri(url)); //获取servertime和 nonce
                webRequest1.CookieContainer = cc;
                webRequest1.Accept = "image/webp,*/*;q=0.8";
                webRequest1.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
                webRequest1.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                webRequest1.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36";
                webRequest1.Referer = "http://weibo.com/";
                webRequest1.Host = "login.sina.com.cn";
                HttpWebResponse response1 = (HttpWebResponse)webRequest1.GetResponse();
            Stream  stream= response1.GetResponseStream();
            if(response1.ContentEncoding.Contains("gzip"))
                stream = new GZipStream(stream, CompressionMode.Decompress);
                Image image = new Bitmap(stream);
                image.Save(savePath+".jpg",ImageFormat.Jpeg);
                Console.WriteLine("请输入验证码");
                response1.Close();
            return Console.ReadLine();
        }
        public double MathRandom()
        {
            Random rnd = new Random();
            return rnd.NextDouble();
        }
        public static string GetResponseByZipStream(HttpWebResponse response1)
        {
            GZipStream g = new GZipStream(response1.GetResponseStream(), CompressionMode.Decompress);
            StreamReader test = new StreamReader(g, Encoding.UTF8);
            return test.ReadToEnd();
        }
        //调用新浪脚本的加密代码
        private static string GetPwds(string psw, string servertime, string nonce, string pubkey)
        {
            string js = "var sinaSSOEncoder=sinaSSOEncoder||{};(function(){var hexcase=0;var chrsz=8;this.hex_sha1=function(s){return binb2hex(core_sha1(str2binb(s),s.length*chrsz));};var core_sha1=function(x,len){x[len>>5]|=0x80<<(24-len%32);x[((len+64>>9)<<4)+15]=len;var w=Array(80);var a=1732584193;var b=-271733879;var c=-1732584194;var d=271733878;var e=-1009589776;for(var i=0;i<x.length;i+=16){var olda=a;var oldb=b;var oldc=c;var oldd=d;var olde=e;for(var j=0;j<80;j++){if(j<16)w[j]=x[i+j];else w[j]=rol(w[j-3]^w[j-8]^w[j-14]^w[j-16],1);var t=safe_add(safe_add(rol(a,5),sha1_ft(j,b,c,d)),safe_add(safe_add(e,w[j]),sha1_kt(j)));e=d;d=c;c=rol(b,30);b=a;a=t;}a=safe_add(a,olda);b=safe_add(b,oldb);c=safe_add(c,oldc);d=safe_add(d,oldd);e=safe_add(e,olde);}return Array(a,b,c,d,e);};var sha1_ft=function(t,b,c,d){if(t<20)return(b&c)|((~b)&d);if(t<40)return b^c^d;if(t<60)return(b&c)|(b&d)|(c&d);return b^c^d;};var sha1_kt=function(t){return(t<20)?1518500249:(t<40)?1859775393:(t<60)?-1894007588:-899497514;};var safe_add=function(x,y){var lsw=(x&0xFFFF)+(y&0xFFFF);var msw=(x>>16)+(y>>16)+(lsw>>16);return(msw<<16)|(lsw&0xFFFF);};var rol=function(num,cnt){return(num<<cnt)|(num>>>(32-cnt));};var str2binb=function(str){var bin=Array();var mask=(1<<chrsz)-1;for(var i=0;i<str.length*chrsz;i+=chrsz)bin[i>>5]|=(str.charCodeAt(i/chrsz)&mask)<<(24-i%32);return bin;};var binb2hex=function(binarray){var hex_tab=hexcase?'0123456789ABCDEF':'0123456789abcdef';var str='';for(var i=0;i<binarray.length*4;i++){str+=hex_tab.charAt((binarray[i>>2]>>((3-i%4)*8+4))&0xF)+hex_tab.charAt((binarray[i>>2]>>((3-i%4)*8))&0xF);}return str;};this.base64={encode:function(input){input=''+input;if(input=='')return '';var output='';var chr1,chr2,chr3='';var enc1,enc2,enc3,enc4='';var i=0;do{chr1=input.charCodeAt(i++);chr2=input.charCodeAt(i++);chr3=input.charCodeAt(i++);enc1=chr1>>2;enc2=((chr1&3)<<4)|(chr2>>4);enc3=((chr2&15)<<2)|(chr3>>6);enc4=chr3&63;if(isNaN(chr2)){enc3=enc4=64;}else if(isNaN(chr3)){enc4=64;}output=output+this._keys.charAt(enc1)+this._keys.charAt(enc2)+this._keys.charAt(enc3)+this._keys.charAt(enc4);chr1=chr2=chr3='';enc1=enc2=enc3=enc4='';}while(i<input.length);return output;},_keys:'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/='};}).call(sinaSSOEncoder);;(function(){var dbits;var canary=0xdeadbeefcafe;var j_lm=((canary&0xffffff)==0xefcafe);function BigInteger(a,b,c){if(a!=null)if('number'==typeof a)this.fromNumber(a,b,c);else if(b==null && 'string' !=typeof a)this.fromString(a,256);else this.fromString(a,b);}function nbi(){return new BigInteger(null);}function am1(i,x,w,j,c,n){while(--n>=0){var v=x*this[i++]+w[j]+c;c=Math.floor(v/0x4000000);w[j++]=v&0x3ffffff;}return c;}function am2(i,x,w,j,c,n){var xl=x&0x7fff,xh=x>>15;while(--n>=0){var l=this[i]&0x7fff;var h=this[i++]>>15;var m=xh*l+h*xl;l=xl*l+((m&0x7fff)<<15)+w[j]+(c&0x3fffffff);c=(l>>>30)+(m>>>15)+xh*h+(c>>>30);w[j++]=l&0x3fffffff;}return c;}function am3(i,x,w,j,c,n){var xl=x&0x3fff,xh=x>>14;while(--n>=0){var l=this[i]&0x3fff;var h=this[i++]>>14;var m=xh*l+h*xl;l=xl*l+((m&0x3fff)<<14)+w[j]+c;c=(l>>28)+(m>>14)+xh*h;w[j++]=l&0xfffffff;}return c;}BigInteger.prototype.am=am3;dbits=28;BigInteger.prototype.DB=dbits;BigInteger.prototype.DM=((1<<dbits)-1);BigInteger.prototype.DV=(1<<dbits);var BI_FP=52;BigInteger.prototype.FV=Math.pow(2,BI_FP);BigInteger.prototype.F1=BI_FP-dbits;BigInteger.prototype.F2=2*dbits-BI_FP;var BI_RM='0123456789abcdefghijklmnopqrstuvwxyz';var BI_RC=new Array();var rr,vv;rr='0'.charCodeAt(0);for(vv=0;vv<=9;++vv)BI_RC[rr++]=vv;rr='a'.charCodeAt(0);for(vv=10;vv<36;++vv)BI_RC[rr++]=vv;rr='A'.charCodeAt(0);for(vv=10;vv<36;++vv)BI_RC[rr++]=vv;function int2char(n){return BI_RM.charAt(n);}function intAt(s,i){var c=BI_RC[s.charCodeAt(i)];return(c==null)?-1:c;}function bnpCopyTo(r){for(var i=this.t-1;i>=0;--i)r[i]=this[i];r.t=this.t;r.s=this.s;}function bnpFromInt(x){this.t=1;this.s=(x<0)?-1:0;if(x>0)this[0]=x;else if(x<-1)this[0]=x+DV;else this.t=0;}function nbv(i){var r=nbi();r.fromInt(i);return r;}function bnpFromString(s,b){var k;if(b==16)k=4;else if(b==8)k=3;else if(b==256)k=8;else if(b==2)k=1;else if(b==32)k=5;else if(b==4)k=2;else{this.fromRadix(s,b);return;}this.t=0;this.s=0;var i=s.length,mi=false,sh=0;while(--i>=0){var x=(k==8)?s[i]&0xff:intAt(s,i);if(x<0){if(s.charAt(i)=='-')mi=true;continue;}mi=false;if(sh==0)this[this.t++]=x;else if(sh+k>this.DB){this[this.t-1]|=(x&((1<<(this.DB-sh))-1))<<sh;this[this.t++]=(x>>(this.DB-sh));}else "
                  + " this[this.t-1]|=x<<sh;sh+=k;if(sh>=this.DB)sh-=this.DB;}if(k==8&&(s[0]&0x80)!=0){this.s=-1;if(sh>0)this[this.t-1]|=((1<<(this.DB-sh))-1)<<sh;}this.clamp();if(mi)BigInteger.ZERO.subTo(this,this);}function bnpClamp(){var c=this.s&this.DM;while(this.t>0&&this[this.t-1]==c)--this.t;}function bnToString(b){if(this.s<0)return '-'+this.negate().toString(b);var k;if(b==16)k=4;else if(b==8)k=3;else if(b==2)k=1;else if(b==32)k=5;else if(b==4)k=2;else return this.toRadix(b);var km=(1<<k)-1,d,m=false,r='',i=this.t;var p=this.DB-(i*this.DB)%k;if(i-->0){if(p<this.DB&&(d=this[i]>>p)>0){m=true;r=int2char(d);}while(i>=0){if(p<k){d=(this[i]&((1<<p)-1))<<(k-p);d|=this[--i]>>(p+=this.DB-k);}else{d=(this[i]>>(p-=k))&km;if(p<=0){p+=this.DB;--i;}}if(d>0)m=true;if(m)r+=int2char(d);}}return m?r:'0';}function bnNegate(){var r=nbi();BigInteger.ZERO.subTo(this,r);return r;}function bnAbs(){return(this.s<0)?this.negate():this;}function bnCompareTo(a){var r=this.s-a.s;if(r!=0)return r;var i=this.t;r=i-a.t;if(r!=0)return r;while(--i>=0)if((r=this[i]-a[i])!=0)return r;return 0;}function nbits(x){var r=1,t;if((t=x>>>16)!=0){x=t;r+=16;}if((t=x>>8)!=0){x=t;r+=8;}if((t=x>>4)!=0){x=t;r+=4;}if((t=x>>2)!=0){x=t;r+=2;}if((t=x>>1)!=0){x=t;r+=1;}return r;}function bnBitLength(){if(this.t<=0)return 0;return this.DB*(this.t-1)+nbits(this[this.t-1]^(this.s&this.DM));}function bnpDLShiftTo(n,r){var i;for(i=this.t-1;i>=0;--i)r[i+n]=this[i];for(i=n-1;i>=0;--i)r[i]=0;r.t=this.t+n;r.s=this.s;}function bnpDRShiftTo(n,r){for(var i=n;i<this.t;++i)r[i-n]=this[i];r.t=Math.max(this.t-n,0);r.s=this.s;}function bnpLShiftTo(n,r){var bs=n%this.DB;var cbs=this.DB-bs;var bm=(1<<cbs)-1;var ds=Math.floor(n/this.DB),c=(this.s<<bs)&this.DM,i;for(i=this.t-1;i>=0;--i){r[i+ds+1]=(this[i]>>cbs)|c;c=(this[i]&bm)<<bs;}for(i=ds-1;i>=0;--i)r[i]=0;r[ds]=c;r.t=this.t+ds+1;r.s=this.s;r.clamp();}function bnpRShiftTo(n,r){r.s=this.s;var ds=Math.floor(n/this.DB);if(ds>=this.t){r.t=0;return;}var bs=n%this.DB;var cbs=this.DB-bs;var bm=(1<<bs)-1;r[0]=this[ds]>>bs;for(var i=ds+1;i<this.t;++i){r[i-ds-1]|=(this[i]&bm)<<cbs;r[i-ds]=this[i]>>bs;}if(bs>0)r[this.t-ds-1]|=(this.s&bm)<<cbs;r.t=this.t-ds;r.clamp();}function bnpSubTo(a,r){var i=0,c=0,m=Math.min(a.t,this.t);while(i<m){c+=this[i]-a[i];r[i++]=c&this.DM;c>>=this.DB;}if(a.t<this.t){c-=a.s;while(i<this.t){c+=this[i];r[i++]=c&this.DM;c>>=this.DB;}c+=this.s;}else{c+=this.s;while(i<a.t){c-=a[i];r[i++]=c&this.DM;c>>=this.DB;}c-=a.s;}r.s=(c<0)?-1:0;if(c<-1)r[i++]=this.DV+c;else if(c>0)r[i++]=c;r.t=i;r.clamp();}function bnpMultiplyTo(a,r){var x=this.abs(),y=a.abs();var i=x.t;r.t=i+y.t;while(--i>=0)r[i]=0;for(i=0;i<y.t;++i)r[i+x.t]=x.am(0,y[i],r,i,0,x.t);r.s=0;r.clamp();if(this.s!=a.s)BigInteger.ZERO.subTo(r,r);}function bnpSquareTo(r){var x=this.abs();var i=r.t=2*x.t;while(--i>=0)r[i]=0;for(i=0;i<x.t-1;++i){var c=x.am(i,x[i],r,2*i,0,1);if((r[i+x.t]+=x.am(i+1,2*x[i],r,2*i+1,c,x.t-i-1))>=x.DV){r[i+x.t]-=x.DV;r[i+x.t+1]=1;}}if(r.t>0)r[r.t-1]+=x.am(i,x[i],r,2*i,0,1);r.s=0;r.clamp();}function bnpDivRemTo(m,q,r){var pm=m.abs();if(pm.t<=0)return;var pt=this.abs();if(pt.t<pm.t){if(q!=null)q.fromInt(0);if(r!=null)this.copyTo(r);return;}if(r==null)r=nbi();var y=nbi(),ts=this.s,ms=m.s;var nsh=this.DB-nbits(pm[pm.t-1]);if(nsh>0){pm.lShiftTo(nsh,y);pt.lShiftTo(nsh,r);}else{pm.copyTo(y);pt.copyTo(r);}var ys=y.t;var y0=y[ys-1];if(y0==0)return;var yt=y0*(1<<this.F1)+((ys>1)?y[ys-2]>>this.F2:0);var d1=this.FV/yt,d2=(1<<this.F1)/yt,e=1<<this.F2;var i=r.t,j=i-ys,t=(q==null)?nbi():q;y.dlShiftTo(j,t);if(r.compareTo(t)>=0){r[r.t++]=1;r.subTo(t,r);}BigInteger.ONE.dlShiftTo(ys,t);t.subTo(y,y);while(y.t<ys)y[y.t++]=0;while(--j>=0){var qd=(r[--i]==y0)?this.DM:Math.floor(r[i]*d1+(r[i-1]+e)*d2);if((r[i]+=y.am(0,qd,r,j,0,ys))<qd){y.dlShiftTo(j,t);r.subTo(t,r);while(r[i]<--qd)r.subTo(t,r);}}if(q!=null){r.drShiftTo(ys,q);if(ts!=ms)BigInteger.ZERO.subTo(q,q);}r.t=ys;r.clamp();if(nsh>0)r.rShiftTo(nsh,r);if(ts<0)BigInteger.ZERO.subTo(r,r);}function bnMod(a){var r=nbi();this.abs().divRemTo(a,null,r);if(this.s<0&&r.compareTo(BigInteger.ZERO)>0)a.subTo(r,r);return r;}function Classic(m){this.m=m;}function cConvert(x){if(x.s<0||x.compareTo(this.m)>=0)return x.mod(this.m);else return x;}function cRevert(x){return x;}function cReduce(x){x.divRemTo(this.m,null,x);}function cMulTo(x,y,r){x.multiplyTo(y,r);this.reduce(r);}function cSqrTo(x,r){x.squareTo(r);this.reduce(r);}Classic.prototype.convert=cConvert;Classic.prototype.revert=cRevert;Classic.prototype.reduce=cReduce;Classic.prototype.mulTo=cMulTo;Classic.prototype.sqrTo=cSqrTo;function bnpInvDigit(){if(this.t<1)return 0;var x=this[0];if((x&1)==0)return 0;var y=x&3;y=(y*(2-(x&0xf)*y))&0xf;y=(y*(2-(x&0xff)*y))&0xff;y=(y*(2-(((x&0xffff)*y)&0xffff)))&0xffff;y=(y*(2-x*y%this.DV))%this.DV;return(y>0)?this.DV-y:-y;}function Montgomery(m){this.m=m;this.mp=m.invDigit();this.mpl=this.mp&0x7fff;this.mph=this.mp>>15;this.um=(1<<(m.DB-15))-1;this.mt2=2*m.t;}function montConvert(x){var r=nbi();x.abs().dlShiftTo(this.m.t,r);r.divRemTo(this.m,null,r);if(x.s<0&&r.compareTo(BigInteger.ZERO)>0)this.m.subTo(r,r);return r;}function montRevert(x){var r=nbi();x.copyTo(r);this.reduce(r);return r;}function montReduce(x){while(x.t<=this.mt2)x[x.t++]=0;for(var i=0;i<this.m.t;++i){var j=x[i]&0x7fff;var u0=(j*this.mpl+(((j*this.mph+(x[i]>>15)*this.mpl)&this.um)<<15))&x.DM;j=i+this.m.t;x[j]+=this.m.am(0,u0,x,i,0,this.m.t);while(x[j]>=x.DV){x[j]-=x.DV;x[++j]++;}}x.clamp();x.drShiftTo(this.m.t,x);if(x.compareTo(this.m)>=0)x.subTo(this.m,x);}function montSqrTo(x,r){x.squareTo(r);this.reduce(r);}function montMulTo(x,y,r){x.multiplyTo(y,r);this.reduce(r);}Montgomery.prototype.convert=montConvert;Montgomery.prototype.revert=montRevert;Montgomery.prototype.reduce=montReduce;Montgomery.prototype.mulTo=montMulTo;Montgomery.prototype.sqrTo=montSqrTo;function bnpIsEven(){return((this.t>0)?(this[0]&1):this.s)==0;}function bnpExp(e,z){if(e>0xffffffff||e<1)return BigInteger.ONE;var r=nbi(),r2=nbi(),g=z.convert(this),i=nbits(e)-1;g.copyTo(r);while(--i>=0){z.sqrTo(r,r2);if((e&(1<<i))>0)z.mulTo(r2,g,r);else{var t=r;r=r2;r2=t;}}return z.revert(r);}function bnModPowInt(e,m){var z;if(e<256||m.isEven())z=new Classic(m);else z=new Montgomery(m);return this.exp(e,z);}BigInteger.prototype.copyTo=bnpCopyTo;BigInteger.prototype.fromInt=bnpFromInt;BigInteger.prototype.fromString=bnpFromString;BigInteger.prototype.clamp=bnpClamp;BigInteger.prototype.dlShiftTo=bnpDLShiftTo;BigInteger.prototype.drShiftTo=bnpDRShiftTo;BigInteger.prototype.lShiftTo=bnpLShiftTo;BigInteger.prototype.rShiftTo=bnpRShiftTo;BigInteger.prototype.subTo=bnpSubTo;BigInteger.prototype.multiplyTo=bnpMultiplyTo;BigInteger.prototype.squareTo=bnpSquareTo;BigInteger.prototype.divRemTo=bnpDivRemTo;BigInteger.prototype.invDigit=bnpInvDigit;BigInteger.prototype.isEven=bnpIsEven;BigInteger.prototype.exp=bnpExp;BigInteger.prototype.toString=bnToString;BigInteger.prototype.negate=bnNegate;BigInteger.prototype.abs=bnAbs;BigInteger.prototype.compareTo=bnCompareTo;BigInteger.prototype.bitLength=bnBitLength;BigInteger.prototype.mod=bnMod;BigInteger.prototype.modPowInt=bnModPowInt;BigInteger.ZERO=nbv(0);BigInteger.ONE=nbv(1);function Arcfour(){this.i=0;this.j=0;this.S=new Array();}function ARC4init(key){var i,j,t;for(i=0;i<256;++i)this.S[i]=i;j=0;for(i=0;i<256;++i){j=(j+this.S[i]+key[i%key.length])&255;t=this.S[i];this.S[i]=this.S[j];this.S[j]=t;}this.i=0;this.j=0;}function ARC4next(){var t;this.i=(this.i+1)&255;this.j=(this.j+this.S[this.i])&255;t=this.S[this.i];this.S[this.i]=this.S[this.j];this.S[this.j]=t;return this.S[(t+this.S[this.i])&255];}Arcfour.prototype.init=ARC4init;Arcfour.prototype.next=ARC4next;function prng_newstate(){return new Arcfour();}var rng_psize=256;var rng_state;var rng_pool;var rng_pptr;function rng_seed_int(x){rng_pool[rng_pptr++]^=x&255;rng_pool[rng_pptr++]^=(x>>8)&255;rng_pool[rng_pptr++]^=(x>>16)&255;rng_pool[rng_pptr++]^=(x>>24)&255;if(rng_pptr>=rng_psize)rng_pptr-=rng_psize;}function rng_seed_time(){rng_seed_int(new Date().getTime());}if(rng_pool==null){rng_pool=new Array();rng_pptr=0;var t;while(rng_pptr<rng_psize){t=Math.floor(65536*Math.random());rng_pool[rng_pptr++]=t>>>8;rng_pool[rng_pptr++]=t&255;}rng_pptr=0;rng_seed_time();}function rng_get_byte(){if(rng_state==null){rng_seed_time();rng_state=prng_newstate();rng_state.init(rng_pool);for(rng_pptr=0;rng_pptr<rng_pool.length;++rng_pptr)rng_pool[rng_pptr]=0;rng_pptr=0;}return rng_state.next();}function rng_get_bytes(ba){var i;for(i=0;i<ba.length;++i)ba[i]=rng_get_byte();}function SecureRandom(){}SecureRandom.prototype.nextBytes=rng_get_bytes;function parseBigInt(str,r){return new BigInteger(str,r);}function linebrk(s,n){var ret='';var i=0;while(i+n<s.length){ret+=s.substring(i,i+n)+'\\n';i+=n;}return ret+s.substring(i,s.length);}function byte2Hex(b){if(b<0x10)return '0'+b.toString(16);else "
                  + " return b.toString(16);}function pkcs1pad2(s,n){if(n<s.length+11){return null;}var ba=new Array();var i=s.length-1;while(i>=0&&n>0){var c=s.charCodeAt(i--);if(c<128){ba[--n]=c;}else if((c>127)&&(c<2048)){ba[--n]=(c&63)|128;ba[--n]=(c>>6)|192;}else{ba[--n]=(c&63)|128;ba[--n]=((c>>6)&63)|128;ba[--n]=(c>>12)|224;}}ba[--n]=0;var rng=new SecureRandom();var x=new Array();while(n>2){x[0]=0;while(x[0]==0)rng.nextBytes(x);ba[--n]=x[0];}ba[--n]=2;ba[--n]=0;return new BigInteger(ba);}function RSAKey(){this.n=null;this.e=0;this.d=null;this.p=null;this.q=null;this.dmp1=null;this.dmq1=null;this.coeff=null;}function RSASetPublic(N,E){if(N!=null&&E!=null&&N.length>0&&E.length>0){this.n=parseBigInt(N,16);this.e=parseInt(E,16);}else"
                     + " alert('Invalid RSA public key');}function RSADoPublic(x){return x.modPowInt(this.e,this.n);}function RSAEncrypt(text){var m=pkcs1pad2(text,(this.n.bitLength()+7)>>3);if(m==null)return null;var c=this.doPublic(m);if(c==null)return null;var h=c.toString(16);if((h.length&1)==0)return h;else return '0'+h;}RSAKey.prototype.doPublic=RSADoPublic;RSAKey.prototype.setPublic=RSASetPublic;RSAKey.prototype.encrypt=RSAEncrypt;this.RSAKey=RSAKey;}).call(sinaSSOEncoder);"
               +//自定义方法 
               "function getpass(pwd,servicetime,nonce,rsaPubkey){var RSAKey=new sinaSSOEncoder.RSAKey();RSAKey.setPublic(rsaPubkey,'10001');var password=RSAKey.encrypt([servicetime,nonce].join('\\t')+'\\n'+pwd);return password;}"; ;
            MSScriptControl.ScriptControl se = new ScriptControl();

            se.Language = "JavaScript";
            se.AddCode(js);
            object obj = se.Run("getpass", new object[] { psw, servertime, nonce, pubkey });
            return obj.ToString();
        }
        //base64加密
        public static string Base64Code(string Message)
        {
            char[] Base64Code = new char[]{'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T', 
         'U','V','W','X','Y','Z','a','b','c','d','e','f','g','h','i','j','k','l','m','n', 
         'o','p','q','r','s','t','u','v','w','x','y','z','0','1','2','3','4','5','6','7', 
         '8','9','+','/','='};
            byte empty = (byte)0;
            System.Collections.ArrayList byteMessage = new System.Collections.ArrayList(System.Text.Encoding.Default.GetBytes(Message));
            System.Text.StringBuilder outmessage;
            int messageLen = byteMessage.Count;
            int page = messageLen / 3;
            int use = 0;
            if ((use = messageLen % 3) > 0)
            {
                for (int i = 0; i < 3 - use; i++)
                    byteMessage.Add(empty);
                page++;
            }
            outmessage = new System.Text.StringBuilder(page * 4);
            for (int i = 0; i < page; i++)
            {
                byte[] instr = new byte[3];
                instr[0] = (byte)byteMessage[i * 3];
                instr[1] = (byte)byteMessage[i * 3 + 1];
                instr[2] = (byte)byteMessage[i * 3 + 2];
                int[] outstr = new int[4];
                outstr[0] = instr[0] >> 2;
                outstr[1] = ((instr[0] & 0x03) << 4) ^ (instr[1] >> 4);
                if (!instr[1].Equals(empty))
                    outstr[2] = ((instr[1] & 0x0f) << 2) ^ (instr[2] >> 6);
                else
                    outstr[2] = 64;
                if (!instr[2].Equals(empty))
                    outstr[3] = (instr[2] & 0x3f);
                else
                    outstr[3] = 64;
                outmessage.Append(Base64Code[outstr[0]]);
                outmessage.Append(Base64Code[outstr[1]]);
                outmessage.Append(Base64Code[outstr[2]]);
                outmessage.Append(Base64Code[outstr[3]]);
            }
            return outmessage.ToString();
        }
        // sha-1加密
        public static string hex_sha1(string Source_String)
        {
            byte[] StrRes = Encoding.Default.GetBytes(Source_String);
            HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
            StrRes = iSHA.ComputeHash(StrRes);
            StringBuilder EnText = new StringBuilder();
            foreach (byte iByte in StrRes)
            {
                EnText.AppendFormat("{0:x2}", iByte);
            }
            return EnText.ToString();
        }

    }
}
