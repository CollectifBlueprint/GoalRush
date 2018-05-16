Player = {
	Color = {R = 255, G = 255, B = 255, A = 255},
	ControllerType = "GamepadAndKeyboard",
	

	Radius = 22,
	RateOfFire = 3,
	FirePower = 400,
	MagnetPower = 2000,
	MagnetRadiusMin = 3, -- depreciated
	MagnetRadiusMax = 8,
	MagnetRadiusMinForTeam = 20, -- depreciated
	MagnetRadiusMaxForTeam = 60,
	MagnetCooldownMS = 300,
	BallSnapRadius = 3;
	ShotImpulse = 400,

	StunTimeMSDefault = 500,
	StunSpeedModDefault = 0.1,
	StunBallImpulseDefault = 0,

	Controls = {
		Agility = 600.0,
		Speed = 4600,
		SpeedWithBallMod = 1.0,
		BallSpeed = 20
	},

	Physic = {
		LinearDamping = 10,
		AngularDamping = 0.2,
		Restitution = 0.5,
		Mass = 7,
	},

	Skills = {
		Fire = false,
		Tackle = true,
	},

	Tackle = "@Tackle.lua::Tackle",
	Shield = "@Shield.lua::Shield",
	ChargedShot = "@ChargedShot.lua::ChargedShot",
	ChargedPass = "@ChargedPass.lua::ChargedPass",
}


function _deepcopy(t)
if type(t) ~= 'table' then return t end
local mt = getmetatable(t)
local res = {}
for k,v in pairs(t) do
if type(v) == 'table' then
v = _deepcopy(v)
end
res[k] = v
end
setmetatable(res,mt)
return res
end


PlayerDefault = {
	Player = _deepcopy(Player),
}

PlayerDefaultAI = {
	Player = _deepcopy(Player),
}
PlayerDefaultAI.Player.ControllerType = "AI"
PlayerDefaultAI.Player.Controls.BallSpeed = 6


PlayerDefaultHuman = {
	Player = _deepcopy(Player),
}
PlayerDefaultHuman.Player.ControllerType = "Gamepad"
PlayerDefaultHuman.Player.Skills.Fire = false
PlayerDefaultHuman.Player.Skills.Tackle = true

PlayerHuman1 = {
	Player = _deepcopy(PlayerDefaultHuman.Player),
}
PlayerHuman1.Player.Color = {R = 252, G = 25, B = 25, A = 255}
PlayerHuman1.Player.ControllerType = "GamepadAndKeyboard"


PlayerHuman2 = {
	Player = _deepcopy(PlayerDefaultHuman.Player),
}
PlayerHuman2.Player.Color = {R = 230, G = 223, B = 23, A = 255}
PlayerHuman2.Player.ControllerType = "Gamepad"


PlayerHuman3 = {
	Player = _deepcopy(PlayerDefaultHuman.Player),
}
PlayerHuman3.Player.Color = {R = 199, G = 23, B = 230, A = 255}
PlayerHuman3.Player.ControllerType = "Gamepad"

PlayerHuman4 = {
	Player = _deepcopy(PlayerDefaultHuman.Player),
}
PlayerHuman4.Player.Color = {R = 23, G = 192, B = 230, A = 255}
PlayerHuman4.Player.ControllerType = "Gamepad"

PlayerAI1 = {
	Player = _deepcopy(PlayerDefaultAI.Player),
}
PlayerAI1.Player.Color = {R = 199, G = 23, B = 230, A = 255}

PlayerAI2 = {
	Player = _deepcopy(PlayerDefaultAI.Player),
}
PlayerAI2.Player.Color = {R = 23, G = 192, B = 230, A = 255}


