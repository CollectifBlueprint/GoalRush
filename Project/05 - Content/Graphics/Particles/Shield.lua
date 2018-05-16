Emitter = {
	Name = "Shield",
	
	Shape = { _class = "ConeShape", Angle = 1.6 },
	BlendMode = "Dual",
	Coordinates = "Local",

	Modifiers = {
		--{ _class = "FadeIn", Time = 0.2, },
		{ _class = "ScaleOverLifetime", Time = 0.2},
		--{ _class = "Stretch", Strength = 0}, 
		--{ _class = "Curl", Strength = 0.4 },
		--{ _class = "FadeOut", Time = 0.0, },
		{ _class = "ColorFadeOut", Time = -0.2, Color = {R = 100, G = 100, B = 100}, },
	},

	MaxParticle = 2000,
	EmitRate = 200,

	Duration = 250,

	Scale = { Value = { X = 0.15, Y = 0.5 } },
	Color = {R = 120, G = 50, B = 45}, 
	Lifetime = 0.3,
	Position = { Value = { X = 24, Y = 0 } , Variation = {X = 2, Y = 10}},
	Velocity = { Value = { X = 0.3, Y = 0 } , Variation = {X = 0, Y = 0.0}},
	Opacity = 1,
}