Sprite = {
	Animations = {
		{
			Name = "Sparks",
			Texture = "@Graphics/playerSparks.png",
			FrameCount = { X = 12, Y = 1 },
			StartIndex = 0,
			EndIndex = 10,
			FrameTime = 85,
			Loop = true, 
		},

		{
			Name = "GoldFx",
			Texture = "@Graphics/playerGoldFx.png",
			FrameCount = { X = 4, Y = 4 },
			StartIndex = 0,
			EndIndex = 15,
			FrameTime = 40,
			Loop = false, 
		},
		
		{
			Name = "ShieldUp",
			Texture = "@Graphics/playerShieldUp.png",
			FrameCount = { X = 4, Y = 4 },
			StartIndex = 0,
			EndIndex = 15,
			FrameTime = 25,
			Loop = false, 
		},
	},	
	
	DefaultAnimation = "Sparks",		
	Scale = 0.48,
}


