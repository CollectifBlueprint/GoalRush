Preview = {
	Name = "Arena 1",
	Description = "The regular matchup arena",

	Preview = "@Arenas/ArenaLarge/Preview.png",

	Size = "Large",
}

Description = {
	Script = { _class = "ArenaLarge2" },

	Material = "@Arenas/ArenaLarge/material.png",
	AO = "@Arenas/ArenaLarge/AO.png",
	
	Geometry = "@Arenas/ArenaLarge/collision.png",

	Size = { X = 2000, Y = 800},
	Scale = 0.5,

	LD =  "@Arenas/ArenaLarge/Arena.lua::LD",
	LDSettings =  "@Arenas/ArenaLarge/Arena.lua::LDSettings",
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
