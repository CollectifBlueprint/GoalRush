Emitter = {
	Name = "Charge",
	
	Shape = { _class = "CircleShape" },
	BlendMode = "Blend",
	Coordinates = "Local",

	Modifiers = {
		{ _class = "FadeIn", Time = 0.4, },
		--{ _class = "ScaleOverLifetime", Time = 0.2},
		--{ _class = "Stretch", Strength = 0}, 
		--{ _class = "Curl", Strength = 0.4 },
		--{ _class = "FadeOut", Time = 0.0, },
		{ _class = "ColorFadeOut", Time = -0.2, Color = {R = 60, G = 60, B = 60}, },
	},

	MaxParticle = 2000,
	EmitRate = 200,  

	Scale = { Value = { X = 0.2, Y = 0.2 } },
	Color = {R = 140, G = 140, B = 140}, 
	Lifetime = 0.6,
	Position = { Value = { X = 35, Y = 0 } , Variation = {X = 5, Y = 0}},
	Velocity = { Value = { X = -0.5, Y = 0 } , Variation = {X = 0, Y = 0}},
	Opacity = 1,
}

EmitterHL = {
	Name = "ChargeHL",
	
	Shape = { _class = "CircleShape" },
	BlendMode = "Blend",
	Coordinates = "Local",

	Modifiers = {
		{ _class = "FadeIn", Time = 0.1, },
		{ _class = "ScaleOverLifetime", Time = 0.2},
		--{ _class = "Stretch", Strength = 0}, 
		--{ _class = "Curl", Strength = 0.4 },
		--{ _class = "FadeOut", Time = 0.0, },
		{ _class = "ColorFadeOut", Time = -0.2, Color = {R = 255, G = 255, B = 255}, },
	},

	MaxParticle = 2000,
	EmitRate = 100,  

	Scale = { Value = { X = 0.45, Y = 0.1 } },
	Color = {R = 140, G = 140, B = 140}, 
	Lifetime = 0.3,
	Position = { Value = { X = 30, Y = 0 } , Variation = {X = 0, Y = 0}},
	Velocity = { Value = { X = -0, Y = 0 } , Variation = {X = 0, Y = 0}},
	Opacity = 1,
}