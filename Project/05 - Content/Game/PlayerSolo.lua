Player = {
	Color = {R = 255, G = 255, B = 255, A = 255},
	ControllerType = "GamepadAndKeyboard",

	Radius = 22,

	BallSnapRadius = 3,

	MagnetPower = 2000,
	MagnetRadiusMin = 3,
	MagnetRadiusMax = 8,
	MagnetCooldownMS = 300,

	ShotImpulse = 400,

	StunTimeMSDefault = 500,
	StunSpeedModDefault = 0.1,
	StunBallImpulseDefault = 0,

	Physic = {
		LinearDamping = 10,
		AngularDamping = 0.2,
		Restitution = 0.5,
		Mass = 7,
	},

	Controls = {
		Agility = 600.0,
		Speed = 4600,
		SpeedWithBallMod = 1.0,
		BallSpeed = 20
	},
	
	Tackle = {
		Speed = 1200,
		Power = 0,
		CooldownMS = 500,
		DurationMS = 250,
		StunTimeMS = 1000,
		StunSpeedMod = 0.1,
	},

	Shield = {
		DurationMS = 3000,
		Radius = 45,
	},

	ChargedPass = {
		ChargeTimeMS = 2000,
		PlayerSpeedMod = 0.6,
	},

	ChargedShot = {
		ChargeTimeMS = 400,
		ShotImpulse = 620,
		PlayerSpeedMod = 0.1,
		BallBashTimeMS = 500,
		BallBashImpulse = 1200,
	},

	Skills = {
		--Speed
		SpeedBase = 0.5,
		SpeedCoef = 0.25,

		--Shoot
		ShotPowerBase = 1.0,
		ShotPowerCoef = 0.0,

		ChargedShotPowerBase = 1.0,
		ChargedShotPowerCoef = 0.0,

		ChargedShotTimeBase = 2.0,
		ChargedShotTimeCoef = -0.5,

		ChargedShotCurveBase = 0.33,
		ChargedShotCurveCoef = 0.33,
	
		--Pass
		PassCurveBase = 1.0,
		PassCurveCoef = 0.0,
	},
}