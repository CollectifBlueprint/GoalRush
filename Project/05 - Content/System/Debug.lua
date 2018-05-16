Debug = {
	Commands = {
		{
			Key = "F12",
			Name = "ResetDebugFlags",
			Description = "Reset all the debug flags",

			Script = { _class = "ResetDebugFlags"},
		},	
		{
			Key = "F9",
			Name = "ToggleAI",
			Description = "Toggle the update of AI players",

			Script = { _class = "ToggleAI"},
		},
		{
			Key = "F11",
			Name = "DumpGameObjects",
			Description = "Log all the game objects in the game",

			Script = { _class = "DumpGameObjects"},
		},
		{
			Key = "F5",
			Name = "ToggleRenderSprite",
			Description = "Toggle the visibility of all sprites",

			Script = { _class = "ToggleRenderSprite"},
		},
		{
			Key = "F6",
			Name = "ToggleRenderDebug",
			Description = "Toggle the visibility of debug screen",

			Script = { _class = "ToggleRenderDebug"},
		},
		{
			Key = "F7",
			Name = "TogglePhysicsDebug",
			Description = "Toggle the visibility of physics debug",

			Script = { _class = "TogglePhysicsDebug"},
		},			
		{
			Key = "F10",
			Name = "ToggleSlowPhysics",
			Description = "Toggle the slow mode for physics",

			Script = { _class = "ToggleSlowPhysics"},
		},
		{
			Key = "F4",
			Name = "ToggleColorDebug",
			Description = "Toggle the slow mode for physics",

			Script = { _class = "ToggleColorDebug"},
		},
		{
			Key = "F3",
			Name = "DumpAssets",
			Description = "Write all the assets to a stream",

			Script = { _class = "DumpAssets"},
		},
	}
}