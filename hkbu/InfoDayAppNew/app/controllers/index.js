$.index.open();

function btClick(e){
	console.log("ghggggggg");
	$.mapView.region={
		latitude:22.339468,
longitude:114.181879,
latitudeDelta:0.001,
longitudeDelta:0.001
};
};
function tableClick(e) {
alert("Table Clicked"); 
var eventListController =Alloy.createController('eventList',{fid:e.row.fid});
$.eventTab.open(eventListController.getView());
};




Alloy.Collections.webNews.fetch();

function transformFunction(model)
{
	var transform = model.toJSON();
	if(transform.thumbnail==null)
	{
		transform.thumbnail ="";
	}
	return transform;
}

$.index.open();

function mapClicked(e) {
     if (e.clicksource == 'rightButton' && e.annotation.id == 'acHall')
{
console.log("map Clicked");
alert("map Clicked");

$.index.open();

function btClick(e){
	console.log("ghggggggg");
	$.mapView.region={
		latitude:22.339468,
longitude:114.181879,
latitudeDelta:0.001,
longitudeDelta:0.001
};
};



Alloy.Collections.webNews.fetch();

function transformFunction(model)
{
	var transform = model.toJSON();
	if(transform.thumbnail==null)
	{
		transform.thumbnail ="";
	}
	return transform;
}


function mapClicked(e) {
     if (e.clicksource == 'rightButton' && e.annotation.id == 'acHall')
{
} }
console.log("map Clicked");
alert("map Clicked");
$.index.open();

function btClick(e){
	console.log("ghggggggg");
	$.mapView.region={
		latitude:22.339468,
longitude:114.181879,
latitudeDelta:0.001,
longitudeDelta:0.001
};
};


Alloy.Collections.webNews.fetch();

function transformFunction(model)
{
	var transform = model.toJSON();
	if(transform.thumbnail==null)
	{
		transform.thumbnail ="";
	}
	return transform;
}


function mapClicked(e) {
     if (e.clicksource == 'rightButton' && e.annotation.id == 'acHall')
{
} }
console.log("map Clicked");
alert("map Clicked");
var route = Alloy.Globals.Map.createRoute({
    points: [
           {latitude: 22.341072, longitude: 114.179645},
           {latitude: 22.337832, longitude: 114.181962},
] });
$.mapView.addRoute(route);
}}


function takePicture(e) {
	Titanium.Media.showCamera(
		{
success:function(event) {
             
              Ti.API.debug("Our type was:"+ event.mediaType);
              if(event.mediaType == Ti.Media.MEDIA_TYPE_PHOTO) {
              	 $.imageView.image = event.media;
              } else {
                  alert("got the wrong type back ="+ event.mediaType);
                }
          },
          cancel:function() {
              
          },
          error:function(error) {
             
              var a = Titanium.UI.createAlertDialog({title:'Camera'});
              if (error.code == Titanium.Media.NO_CAMERA) {
                  a.setMessage('Please run this test on device');
              } else {
                  a.setMessage('Unexpected error: ' + error.code);
              }
a.show(); },
          saveToPhotoGallery:true,
         
          allowEditing:true,
          mediaTypes:[Ti.Media.MEDIA_TYPE_VIDEO,Ti.Media.MEDIA_TYPE_PHOTO]
}); }


function loadPicture(e) {
      Titanium.Media.openPhotoGallery({
          success:function(event) {
              if(event.mediaType == Ti.Media.MEDIA_TYPE_PHOTO) {
                  $.imageView.image = event.media;
              } else {
                  alert("got the wrong type back ="+event.mediaType);
} },
          mediaTypes:[Ti.Media.MEDIA_TYPE_VIDEO,Ti.Media.MEDIA_TYPE_PHOTO]
      });
}