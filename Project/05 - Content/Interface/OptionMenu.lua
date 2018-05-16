Menu = {
	Name = "Options",
	Script = { 
		_class = "OptionMenuScript" 
	},

	Items = {
		{
			Name = "Fullscreen",
			Text = "Fullscreen:",

			Type = "Toggle",
		},
		{
			Name = "VSync",
			Text = "VSync:",

			Type = "Toggle",
		},
		{
			Name = "Music",
			Text = "Music:",

			Type = "Slider",
		},	
		{
			Name = "SFX",
			Text = "SFX:",

			Type = "Slider",
		},		
		{
			Name = "Reset",
			Text = "Reset !!!",
		},
		{
			Name = "Accept",
			Text = "Accept",
		},
	},	

	Sounds = "@Interface/MenuSounds.lua::MenuSoundsDefault"
}

SelectedStyle = {
}