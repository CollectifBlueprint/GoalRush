Preview = {
	Name = "Arena 6",
	Description = "Two spinning plateforms driving the shortest path to goals",

	Preview = "@Arenas/ArenaMobLarge/Preview.png",

	Size = "Large",
}

Description = {
	Script = { _class = "ArenaMobLarge" },

	Material = "@Arenas/ArenaMobLarge/material.png",
	AO = "@Arenas/ArenaMobLarge/AO.png",
	
	Geometry = "@Arenas/ArenaMobLarge/collision.png",

	Size = { X = 2000, Y = 800},
	Scale = 0.5,

	LD =  "@Arenas/ArenaMobLarge/Arena.lua::LD",
	LDSettings =  "@Arenas/ArenaMobLarge/Arena.lua::LDSettings",
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
