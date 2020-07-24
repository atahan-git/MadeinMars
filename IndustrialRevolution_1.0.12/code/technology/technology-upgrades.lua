------------------------------------------------------------------------------------------------------------------------------------------------------

local damage_icon = { icon = get_icon_path("damage", 64), icon_size = 64, scale = 0.67, shift = {-42,42} }
local rate_icon = { icon = get_icon_path("rate-of-fire", 64), icon_size = 64, scale = 0.67, shift = {-42,42} }

for i=1,7 do
	local tech = data.raw.technology["physical-projectile-damage-"..i]
	tech.effects = {}
	table.insert(tech.effects, {type = "ammo-damage", modifier = 0.2, ammo_category = "bullet"})
	table.insert(tech.effects, {type = "ammo-damage", modifier = 0.2, ammo_category = "shotgun-shell"})
	if i >= 5 then table.insert(tech.effects, {type = "ammo-damage", modifier = 0.2, ammo_category = "belt"}) end
	table.insert(tech.effects, {type = "turret-attack", modifier = 0.2, turret_id = "gun-turret"})
	table.insert(tech.effects, {type = "turret-attack", modifier = 0.2, turret_id = "scattergun-turret"})
	if i >= 5 then table.insert(tech.effects, {type = "turret-attack", modifier = 0.2, turret_id = "minigun-turret"}) end
	tech.icon = nil
	tech.icons = {
		{ icon = get_icon_path("physical-projectiles", 128), icon_size = 128 },
		damage_icon,
	}
	if i == 7 then
		tech.max_level = "infinite"
		tech.unit.count = nil
	end
end

for i=1,5 do
	local tech = data.raw.technology["weapon-shooting-speed-"..i]
	tech.effects = {
		{
			ammo_category = "bullet",
			modifier = 0.2,
			type = "gun-speed"
		},
		{
			ammo_category = "shotgun-shell",
			modifier = 0.2,
			type = "gun-speed"
		}
	}
	tech.icon = nil
	tech.icons = {
		{ icon = get_icon_path("physical-projectiles", 128), icon_size = 128 },
		rate_icon,
	}
end

for i=1,7 do
	local tech = data.raw.technology["energy-weapons-damage-"..i]
	tech.effects = {
		{
			ammo_category = "laser-turret",
			modifier = (i < 7) and 0.25 or 0.5,
			type = "ammo-damage"
		},
        {
			ammo_category = "combat-robot-laser",
			modifier = (i < 7) and 0.15 or 0.3,
			type = "ammo-damage"
        },
        {
			ammo_category = "combat-robot-beam",
			modifier = (i < 7) and 0.15 or 0.3,
			type = "ammo-damage"
        }
	}
	tech.icon = nil
	tech.icons = {
		{ icon = get_icon_path("energy-weapons-damage", 128), icon_size = 128 },
		damage_icon,
	}
	if i <= 5 then
		table.insert(tech.effects, 2,
			{
				ammo_category = "laser-turret",
				modifier = 0.1,
				type = "gun-speed"
			}
		)
		table.insert(tech.icons, { icon = get_icon_path("rate-of-fire", 64), icon_size = 64, scale = 0.67, shift = {42,42} })
	end
	if i == 7 then
		tech.max_level = "infinite"
		tech.unit.count = nil
	end
end

for i=1,4 do
	local tech = add_new_minimal_tech("deadlock-photon-turret-damage-"..i, "", false, nil, {"deadlock-photon-turret"},
		{
			{type = "ammo-damage", modifier = 0.5, ammo_category = "photon-torpedo"},
		}
	) 
	tech.icon = nil
	tech.icons = {
		{ icon = get_icon_path("photon-turret", 128), icon_size = 128 },
		damage_icon,
	}
	tech.upgrade = true
	if i == 4 then
		tech.max_level = "infinite"
		tech.unit.count = nil
	end
end

for i=1,7 do
	local tech = add_new_minimal_tech("inserter-capacity-bonus-"..i, "stack-inserter", true, nil, {"deadlock-inserters-3"},
		{
			{type = "stack-inserter-capacity-bonus", modifier = 2}
		}
	) 
	tech.upgrade = (i < 4)
	tech.enabled = (i < 4)
end

for i=1,6 do
	local tech = add_new_minimal_tech("deadlock-robot-battery-"..i, "robot-battery", false, nil, {"robotics"},
		{
			{type = "worker-robot-battery", modifier = math.min(0.5, i * 0.1)}
		}
	) 
	tech.upgrade = true
	if i == 6 then
		tech.max_level = "infinite"
		tech.unit.count = nil
	end
end

table.insert(data.raw.technology["stronger-explosives-3"].effects, {type = "ammo-damage", modifier = 0.3, ammo_category = "cannon-shell"})
table.insert(data.raw.technology["stronger-explosives-4"].effects, {type = "ammo-damage", modifier = 0.4, ammo_category = "cannon-shell"})
table.insert(data.raw.technology["stronger-explosives-5"].effects, {type = "ammo-damage", modifier = 0.5, ammo_category = "cannon-shell"})
table.insert(data.raw.technology["stronger-explosives-5"].effects, {type = "ammo-damage", modifier = 0.5, ammo_category = "cannon-shell"})
table.insert(data.raw.technology["stronger-explosives-6"].effects, {type = "ammo-damage", modifier = 0.5, ammo_category = "cannon-shell"})
table.insert(data.raw.technology["stronger-explosives-7"].effects, {type = "ammo-damage", modifier = 0.5, ammo_category = "cannon-shell"})

------------------------------------------------------------------------------------------------------------------------------------------------------
