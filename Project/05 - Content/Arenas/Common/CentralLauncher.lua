CentralLauncher = {
	Name = "Launcher",
	Components = {
		{
			_class = "CentralBallLauncherComponent",
			Transform = {
				Position = { X = 0, Y = 0},
				Orientation = 0,
			},
		},
		{
			_class = "SpriteComponent",	
			Name = "LauncherSprite",
			Parameters = {		
				Sprite = {
					Animations = {
						{
							Name = "Normal",
							Texture = "@Graphics/LauncherCentral.png",
							FrameCount = { X = 3, Y = 1 },
							StartIndex = 0,
							EndIndex = 0,
							FrameTime = 0,
							Loop = true, 
						},
						{
							Name = "Selected",
							Texture = "@Graphics/LauncherCentral.png",
							FrameCount = { X = 3, Y = 1 },
							StartIndex = 1,
							EndIndex = 2,
							FrameTime = 100,
							Loop = true, 
						},
					},	
					DefaultAnimation = "Normal",	
				},
				RenderLayer = "ArenaOverlay6",
			}
		}
	}
}
