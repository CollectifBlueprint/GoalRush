Emitter = {
	Name = "Flamme",
	
	Shape = { _class = "LineShape", Angle = 1.6 },
	Coordinates = "Local",

	Modifiers = {
		--{ _class = "FadeIn", Time = 0.2, },
		--{ _class = "ScaleOverLifetime" },
		--{ _class = "Stretch", Strength = 0}, 
		--{ _class = "Curl", Strength = 0.1 },
		--{ _class = "FadeOut", Time = 0.0, },
		{ _class = "ColorFadeOut", Time = 0.2, Color = {R = 100, G = 80, B = 30}, },
		{ _class = "Gravity", Strength = -0.4 },
	},

	MaxParticle = 2000,
	EmitRate = 120,

	Scale = { Value = { X = 0.2, Y = 0.2 } },
	Color = {R = 230, G = 80, B = 50}, 
	Lifetime = {Value = 0.25, Variation = 0.1},
	Position = { Value = { X = 0, Y = 4 } , Variation = {X = 5, Y = 7}},
	Velocity = { Value = { X = 0, Y = 0.1 } , Variation = {X = 0, Y = 0.5}},
	Opacity = 0.7,
}

Sparlkle = {
	Name = "Flamme",
	
	Shape = { _class = "LineShape" },
	Coordinates = "Local",

	Modifiers = {
		--{ _class = "FadeIn", Time = 0.2, },
		--{ _class = "ScaleOverLifetime" },
		--{ _class = "Stretch", Strength = 0}, 
		{ _class = "Curl", Strength = 0.4 },
		{ _class = "FadeOut", Time = 0.0, },
		{ _class = "Gravity", Strength = 0.15, },
		{ _class = "ColorFadeOut", Time = 0.2, Color = {R = 100, G = 80, B = 30}, },
	},

	MaxParticle = 2000,
	EmitRate = 50,

	Scale = { Value = { X = 0.2, Y = 0.2 } },
	Color = {R = 230, G = 230, B = 230}, 
	Lifetime = {Value = 1.5, Variation = 0.4},
	Position = { Value = { X = 0, Y = 280 } , Variation = {X = 370, Y = 70}},
	Velocity = { Value = { X = 0, Y = 0.3 } , Variation = {X = 0.2, Y = 0.6}},
	Opacity = 0.7,
}