Preview = {
	Name = "Arena 4",
	Description = "Damaging laser devices on the way to each goals",

	Preview = "@Arenas/ArenaLaserLarge/Preview.png",

	Size = "Large",
}

Description = {
	Script = { _class = "LaserLarge" },

	Material = "@Arenas/ArenaLaserLarge/material.png",
	AO = "@Arenas/ArenaLaserLarge/AO.png",
	
	Geometry = "@Arenas/ArenaLaserLarge/collision.png",

	Size = { X = 2000, Y = 800},
	Scale = 0.5,

	LD =  "@Arenas/ArenaLaserLarge/Arena.lua::LD",
	LDSettings =  "@Arenas/ArenaLaserLarge/Arena.lua::LDSettings",
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
