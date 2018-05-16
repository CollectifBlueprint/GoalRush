Menu = {
	Name = "SelectMap",
	Script = { 
		_class = "AltSelectMapScript" 
	},
	
	Items = {
		{
			Name = "Team",
			Text = "Team",
			Enabled = 0,
		},
		{
			Name = "Arena",
			Text = "Arena",
			Enabled = 1,
		},
		{
			Name = "Play",
			Text = "Play",
			Enabled = 0,
		},
	},

	Sounds = "@Interface/MenuSounds.lua::MenuSoundsDefault"
}

Params = {
	    DescriptionPanelSize = 600,
        SelectionPanelSize = 300,

        SeparatorLineOffset = 530,
        SeparatorMargin = 60,

        DescriptionPreviewSize = { X = 640, Y = 360 },
        DescriptionPreviewHeight = 120,
        DescriptionPreviewMargin = 50,

		SelectionPreviewSize = { X = 288, Y = 162 },
		SelectionPreviewMargin = 25,
}