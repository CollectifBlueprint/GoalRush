Menu = {
	Name = "SelectTeam",
	Script = { 
		_class = "SelectTeamMenuScript" 
	},

	Items = {
		{
			Name = "Team",
			Text = "Team",
			Enabled = 1,
		},
		{
			Name = "Arena",
			Text = "Arena",
			Enabled = 0,
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
        Offset = 396,
        PanelWidth = 375,

        ControllersHeight = 100,
        ControllersHeightOffset = 170,
        ControllerTextHeightOffset = 10,
}