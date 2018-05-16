Preview = {
	Name = "Arena 3",
	Description = "Small and dispersed obstacles, good for bumby games",

	Preview = "@Arenas/ArenaLarge3/Preview.png",

	Size = "Large",
}

Description = {
	Script = { _class = "ArenaLarge2" },

	Material = "@Arenas/ArenaLarge3/material.png",
	AO = "@Arenas/ArenaLarge3/AO.png",
	
	Geometry = "@Arenas/ArenaLarge3/collision.png",

	Size = { X = 2000, Y = 800},
	Scale = 0.5,

	LD =  "@Arenas/ArenaLarge3/Arena.lua::LD",
	LDSettings =  "@Arenas/ArenaLarge3/Arena.lua::LDSettings",
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
