Sprite = {
	Animations = {
		{
			Name = "Ball",
			Texture = "@Graphics/palet.png",
		},
	},	
	
	DefaultAnimation = "Ball",		
	Scale = 0.4,
}

SpriteBash = {
	Animations = {
		{
			Name = "Normal",
			Texture = "@Graphics/paletBashFx.png",
			FrameCount = { X = 8, Y = 1 },
			StartIndex = 0,
			EndIndex = 6,
			FrameTime = 60,
			Loop = true, 
		},
	},	
	
	DefaultAnimation = "Normal",		
	Scale = 0.4,
}