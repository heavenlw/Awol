-----------------------------------------------------------------------------------------
--
-- main.lua
--
-----------------------------------------------------------------------------------------

-- Your code here
local animalMaterial = {density =1,friction = 0.2,bounce=0.5};
local structMaterial = {density =2,friction = 0.5};
local groundMaterial = {density =5,friction = 1};
local background = display.newImageRect("paisaje.png",1324,768);
background.x = display.contentCenterX;
background.y = display.contentCenterY;
display.setStatusBar(display.HiddenStatusBar);
local bird = display.newImageRect("cartoon_bird.png",80,80);
bird.x= 192;
bird.y= 512;
local physics = require("physics");
physics.start();
physics.addBody(bird,animalMaterial);
local ground = display.newRect(background.x,672,1366,192);
physics.addBody(ground,"static",groundMaterial);--physics.addBody(ground);
--physics.addBody(ground,"static");
ground.isVisible=false;
local  plank = display.newImageRect("Wood-Plank.png",200,30.5);
plank.x=800;
plank.y=432;
physics.addBody(plank,structMaterial);
local column1 = display.newImageRect("column.png",30,122);
column1.x = 720;
column1.y= 512;
physics.addBody(column1,structMaterial);
local column2 = display.newImageRect("column.png",30,122);
column2.x = 880;
column2.y= 512;
physics.addBody(column2,structMaterial);
local pig = display.newImageRect("Pig-Pink.png",80,74);
pig.x=800;
pig.y= 368;
physics.addBody(pig,animalMaterial);
bird:applyLinearImpulse(200,0.5,bird.x,bird.y);



--ontouch function
bird.x0=bird.x;
bird.Y0=bird.y;
function onTouch(event)
	if event.phase=="began" then
		display.getCurrentStage();setFocus(bird);
		physics.pause();
	elseif
		event.phase=="moveed" then
		local length = math.sqrt((event.x-bird.x0)^2+(event.y-bird.y0)^2);
        local d=length-64;
        if length<=64 then
        	bird.x=event.x;
        	bird.y=event.y;
        	else

		bird.x=(64*event.x+bird.x0*d)/(64+d);
		bird.y=(64*event.y+bird.y0*d)/(64+d);
	end
	elseif
		event.phase=="ended" then
		display.getCurrentStage():setFocus(nil);
		bird:applyLinearImpulse((bird.x0-event.x),(bird.y0-event.y),bird.x,bird.y);
		physics.start();
	end
	-- body
end
bird:addEventListener("touch",onTouch);

function  onPostCollision(event)
	if(event.force>25) then
		pig.setFrame(2);
	
	end
end
pig:addEventListener("postCollision",onPostCollision);

local pig = display.newImageRect("Pig-Pink.png",80,74);

local pigSheet = graphics.newImageSheet("pigs.png",{width=80,height=80,numFrames=2,sheetContentWidth=160,sheetContentHeight=80})
local sequenceData={name="pigs",start=1,count=2};
local pig=display.newSprite(pigSheet,sequenceData);
pig:setSequence("pigs");
pig:setFrame(1);

--music
--local bgMusic = audio.loadSound("Bird-Video-Game.mp3");
audio.play(bgMusic,{channel=1,loops=-1,fadein=50000});
audio.setVolume(0.3,{channel=1});

--pig sound 
function pigSound(event)
--local sound = audio.loadSound("pig.mp3");
audio.play(sound,{channel=2});
end
local timeId = timer.performWithDelay(2000,pigSound,0);
timer.cancel(timeId);

--Scoring
local  score = 0;
function onPostCollision(event)
	if event.force>25 then
		pig.setFrame(2)
		timer.cancel(timeId);

		score=math.round(event.force);
		local scoreText = display.newText("Testing",600,100,nil,32);
		scoreText:setTextColor(255,127,0);
		scoreText = "Score:"..score;
		pig:removeEventListener("postCollision",onPostCollision);
		bird:removeEventListener("touch",onTouch);
		end;
	-- body
end

