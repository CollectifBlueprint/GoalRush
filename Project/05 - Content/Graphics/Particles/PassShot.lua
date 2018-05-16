Ball = {
	Name = "Ball",
	
	Shape = { _class = "ConeShape", Angle = 1.1},

	BlendMode = "Dual",
	
	Coordinates = "Local";

	Modifiers = {
		{ _class = "ScaleOverLifetime", Time = 0.2},
		{ _class = "FadeOut", Time = 0.0, },
		{ _class = "ColorFadeOut", Time = -0.2, Color = {R = 120, G = 120, B = 120}, },
	},

	MaxParticle = 2000,
	EmitRate = 20,  
	EmitDelay = 0, 

	Scale = { Value = { X = 0.2, Y = 0.2 } },
	Color = {R = 200, G = 60, B = 10}, 
	Lifetime = 0.15,
	Position = { Value = { X = 0, Y = 0 } , Variation = {X = 0, Y = 1}},
	Velocity = { Value = { X = -1.5, Y = 0 } , Variation = {X = 0, Y = 0}},
	Opacity = 1,
}

TrailHighlight = {
	Name = "TrailHighlight",
	
	Shape = { _class = "LineShape" },

	BlendMode = "Add",

	Modifiers = {
		{ _class = "FadeIn", Time = 0.1, },
		{ _class = "ScaleOverLifetime", Time = 0.0},
		--{ _class = "Stretch", Strength = 0}, 
		--{ _class = "Curl", Strength = 1.4 },
		{ _class = "FadeOut", Time = 0.0, },
		{ _class = "ColorFadeOut", Time = -0.2, Color = {R = 120, G = 120, B = 120}, },
	},

	MaxParticle = 2000,
	EmitRate = 10,  
	EmitDelay = 0, 

	Scale = { Value = { X = 0.45, Y = 0.05 } },
	Color = {R = 200, G = 60, B = 10}, 
	Lifetime = 0.17,
	Position = { Value = { X = -3, Y = 0 } , Variation = {X = 4, Y = 4}},
	Velocity = { Value = { X = 0, Y = 0 } , Variation = {X = 0.3, Y = 0}},
	Opacity = 0.95,
}


Trail = {
	Name = "Trail",
	
	Shape = { _class = "CircleShape" },

	BlendMode = "Blend",

	Modifiers = {
		{ _class = "FadeIn", Time = 0.1, },
		{ _class = "ScaleOverLifetime", Time = 0.0},
		--{ _class = "Stretch", Strength = 0}, 
		--{ _class = "Curl", Strength = 0.8 },
		{ _class = "FadeOut", Time = 0.0, },
		{ _class = "ColorFadeOut", Time = -0.2, Color = {R = 120, G = 120, B = 120}, },
	},

	MaxParticle = 2000,
	EmitRate = 200,  
	EmitDelay = 0, 

	Scale = { Value = { X = 0.45, Y = 0.25 } },
	Color = {R = 200, G = 60, B = 10}, 
	Lifetime = 0.1,
	Position = { Value = { X = 0, Y = 0 } , Variation = {X = 3, Y = 0}},
	Velocity = { Value = { X = 0, Y = 0 } , Variation = {X = 0, Y = 0}},
	Opacity = 0.6,
}