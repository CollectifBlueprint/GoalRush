Preview = {
	Name = "Arena 5",
	Description = "A tricky laser arena",

	Preview = "@Arenas/ArenaLaserLarge2/Preview.png",

	Size = "Large",
}

Description = {
	Script = { _class = "LaserLarge2" },

	Material = "@Arenas/ArenaLaserLarge2/material.png",
	AO = "@Arenas/ArenaLaserLarge2/AO.png",
	
	Geometry = "@Arenas/ArenaLaserLarge2/collision.png",

	Size = { X = 2000, Y = 800},
	Scale = 0.5,

	LD =  "@Arenas/ArenaLaserLarge2/Arena.lua::LD",
	LDSettings =  "@Arenas/ArenaLaserLarge2/Arena.lua::LDSettings",
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
