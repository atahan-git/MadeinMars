------------------------------------------------------------------------------------------------------------------------------------------------------

local conversion = {
	["advanced-electronics"] = "deadlock-electronics-2",
	["advanced-electronics-2"] = "deadlock-electronics-3",
	["advanced-material-processing"] = "deadlock-furnaces-2",
	["advanced-material-processing-2"] = "deadlock-furnaces-3",
	["concrete"] = "deadlock-concrete-1",
	["effectivity-module"] = "deadlock-modules-1",
	["effectivity-module-2"] = "deadlock-modules-2",
	["effectivity-module-3"] = "deadlock-modules-3",
	["electronics"] = "deadlock-electronics-1",
	["fast-inserter"] = "deadlock-inserters-2",
	["laser-turret-speed-1"] = "energy-weapons-damage-1",
	["laser-turret-speed-2"] = "energy-weapons-damage-2",
	["laser-turret-speed-3"] = "energy-weapons-damage-3",
	["laser-turret-speed-4"] = "energy-weapons-damage-4",
	["laser-turret-speed-5"] = "energy-weapons-damage-5",
	["laser-turret-speed-6"] = "energy-weapons-damage-5",
	["laser-turret-speed-7"] = "energy-weapons-damage-5",
	["modules"] = "deadlock-modules-1",
	["productivity-module"] = "deadlock-modules-1",
	["productivity-module-2"] = "deadlock-modules-2",
	["productivity-module-3"] = "deadlock-modules-3",
	["rail-signals"] = "automated-rail-transportation",
	["solar-energy"] = "deadlock-solar-energy-1",
	["speed-module"] = "deadlock-modules-1",
	["speed-module-2"] = "deadlock-modules-2",
	["speed-module-3"] = "deadlock-modules-3",
	["stack-inserter"] = "deadlock-inserters-3",
	["steel-processing"] = "deadlock-steel-age",
	["uranium-ammo"] = "military-4",
	["weapon-shooting-speed-6"] = "weapon-shooting-speed-5",
	["construction-robotics"] = "personal-roboport-equipment",
	["logistic-robotics"] = "robotics",
}

for _,tech in pairs(data.raw.technology) do
	-- replace any disabled prereqs with closest IR equivalent, where possible
	if tech.prerequisites and tech.enabled ~= false then
		local new = {}
		for _,prereq in pairs(tech.prerequisites) do
			if conversion[prereq] then
				spam_log("Converting prereq for tech "..tech.name..": "..prereq.." > "..conversion[prereq])
				prereq = conversion[prereq]
			end
			table.insert(new,prereq)
		end
		tech.prerequisites = table.deepcopy(new)
	end
	-- search for duplicated prereqs, because other mods may have added the same thing we do, because they are assuming it's just the vanilla tech and they're the only mod in the world
	if tech.prerequisites then
		local new = {}
		for _,prereq in pairs(tech.prerequisites) do
			if not is_value_in_table(new,prereq) then
				table.insert(new,prereq)
			else
				spam_log("Removed duplicated prereq "..prereq.." from tech "..tech.name)
			end
		end
		tech.prerequisites = table.deepcopy(new)
	end
end

------------------------------------------------------------------------------------------------------------------------------------------------------
