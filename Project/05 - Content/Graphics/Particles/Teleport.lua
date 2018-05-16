Emitter = {
	Name = "Teleport",
	
	Shape = { _class = "CircleShape" },
	BlendMode = "Add",
	Coordinates = "Local",

	Modifiers = {
		{ _class = "FadeIn", Time = 0.0, },
		{ _class = "ScaleOverLifetime", Time = 0},
		--{ _class = "Curl", Strength = 0.4 },
		{ _class = "FadeOut", Time = 0.0, },
	},

	MaxParticle = 2000,
	EmitRate = 120,  

	Scale = { Value = { X = 0.05, Y = 0.21 } },
	Color = {R = 220, G = 200, B = 190}, 
	Lifetime = 0.1,
	Position = { Value = { X = 20, Y = 0 } , Variation = {X = 5, Y = 0}},
	Velocity = { Value = { X = -2, Y = 0.2 } , Variation = {X = 0, Y = 0}},
	Opacity = 1,
}