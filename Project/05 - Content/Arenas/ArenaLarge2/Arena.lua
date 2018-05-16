Preview = {
	Name = "Arena 2",
	Description = "An alternative setting with straight goal-to-goal shot actions",

	Preview = "@Arenas/ArenaLarge2/Preview.png",

	Size = "Large",
}

Description = {
	Script = { _class = "ArenaLarge2" },

	Material = "@Arenas/ArenaLarge2/material.png",
	AO = "@Arenas/ArenaLarge2/AO.png",
	
	Geometry = "@Arenas/ArenaLarge2/collision.png",

	Size = { X = 2000, Y = 800},
	Scale = 0.5,

	LD =  "@Arenas/ArenaLarge2/Arena.lua::LD",
	LDSettings =  "@Arenas/ArenaLarge2/Arena.lua::LDSettings",
}


LD = {
	LDObjects = {
		{
			GameObjectDefinition = "@Arenas/Common/CentralLauncher.lua::CentralLauncher",
			Position = { X = 0, Y = 0 },
			Orientation = 0,
			Name = "Launcher"
		},
	}
}

LDSettings = {
	LaunchersSelection = "Central"
}
