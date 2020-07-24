------------------------------------------------------------------------------------------------------------------------------------------------------

require("code.functions.functions-technology")
require("code.functions.functions-recipes")

------------------------------------------------------------------------------------------------------------------------------------------------------

-- TECHNOLOGY WORK-OVER PART 1
-- disable redundant vanilla techs, destroy the entire tree graph, add new techs, add conceptual prerequisites, add science pack prereqs

------------------------------------------------------------------------------------------------------------------------------------------------------

-- unwanted or obsolete vanilla tech

disable_technology("advanced-electronics") --> deadlock-electronics-2
disable_technology("advanced-electronics-2") --> deadlock-electronics-3
disable_technology("advanced-material-processing") --> no material equivalent, functionally this would be deadlock-furnaces-1
disable_technology("advanced-material-processing-2") --> no material equivalent, functionally this would be deadlock-furnaces-2
disable_technology("concrete") --> deadlock-concrete-1
disable_technology("effectivity-module") --> deadlock-modules-1
disable_technology("effectivity-module-2") --> deadlock-modules-2
disable_technology("effectivity-module-3") --> deadlock-modules-3
disable_technology("electronics") --> deadlock-electronics-1
disable_technology("fast-inserter") --> deadlock-inserters-2
disable_technology("flammables") --> no equivalent, unlocks nothing, arguably shouldn't exist
disable_technology("inserter-capacity-bonus-4") --> no equivalent really; in IR, non-stack inserter bonuses are from deadlock-normal-inserter-capacity-bonus-1/2; see also stack-inserter
disable_technology("inserter-capacity-bonus-5") --> inserter-capacity-bonus-1
disable_technology("inserter-capacity-bonus-6") --> inserter-capacity-bonus-2
disable_technology("inserter-capacity-bonus-7") --> inserter-capacity-bonus-3
disable_technology("laser-turret-speed-1") --> energy-weapons-damage-1
disable_technology("laser-turret-speed-2") --> energy-weapons-damage-2
disable_technology("laser-turret-speed-3") --> energy-weapons-damage-3
disable_technology("laser-turret-speed-4") --> energy-weapons-damage-4
disable_technology("laser-turret-speed-5") --> energy-weapons-damage-5
disable_technology("laser-turret-speed-6") --> no equivalent
disable_technology("laser-turret-speed-7") --> no equivalent
disable_technology("modules") --> no equivalent, IR reduces techs involved in unlocking modules from 10 to 3, deadlock-modules-1/2/3
disable_technology("productivity-module") --> deadlock-modules-1
disable_technology("productivity-module-2") --> deadlock-modules-2
disable_technology("productivity-module-3") --> deadlock-modules-3
disable_technology("rail-signals") --> automated-rail-transportation
disable_technology("solar-energy") --> deadlock-solar-energy-1
disable_technology("speed-module") --> deadlock-modules-1
disable_technology("speed-module-2") --> deadlock-modules-2
disable_technology("speed-module-3") --> deadlock-modules-3
disable_technology("stack-inserter") --> deadlock-inserters-3; also adds bonus to stack inserters covered in vanilla by inserter-capacity-bonus-1/2/3/4
disable_technology("steel-axe") --> no equivalent
disable_technology("steel-processing") --> deadlock-steel-age
disable_technology("toolbelt") --> no equivalent, inventory bonuses are purely armour-based in IR
disable_technology("uranium-ammo") --> military-4
disable_technology("weapon-shooting-speed-6") --> no equivalent, IR scales it 1-5
disable_technology("construction-robotics") --> personal-roboport-equipment
disable_technology("logistic-robotics") --> robotics
enable_technology("laser-turrets")

------------------------------------------------------------------------------------------------------------------------------------------------------

-- completely wipe all vanilla prereqs and all science packs. harsh but fair

local vanilla = require("code.data.vanilla-tech")
for _,tech in pairs(data.raw.technology) do
	if tech.enabled ~= false and (does_tech_unlock_recipes(tech) or is_value_in_table(vanilla,tech.name)) then
		tech.prerequisites = nil
		tech.unit.ingredients = {{"automation-science-pack",1}}
		tech.unit.time = 60
		tech.unit.count = 1
		tech.unit.count_formula = nil
	end
end

------------------------------------------------------------------------------------------------------------------------------------------------------

-- initialise non-backbone native techs
require("code.technology.technology-native")

-- run a conversion of hidden tech prerequisites
require("code.technology.technology-prereq-convert")

-- set up mods which need to have ingredients adjusted before the tree rebuild
require("code.mods.mods-data-pretech")

-- adjust vanilla techs
require("code.technology.technology-vanilla")

-- rework weapon and other infinite upgrades
require("code.technology.technology-upgrades")

------------------------------------------------------------------------------------------------------------------------------------------------------

-- re-create chains 

for _,tech in pairs(data.raw.technology) do
	base,_ = string.gsub(tech.name, "-%d$", "")
	if tech.enabled ~= false and (tech.ir_native or does_tech_unlock_recipes(tech) or is_value_in_table(vanilla,tech.name)) then
		for i=10,2,-1 do
			if tech.name == (base.."-"..i) then 
				if i > 2 then
					if data.raw.technology[base.."-"..(i-1)] then add_prereq_to_tech(tech.name, base.."-"..(i-1)) end
				else
					if data.raw.technology[base.."-1"] then add_prereq_to_tech(tech.name, base.."-1")
					elseif data.raw.technology[base] then add_prereq_to_tech(tech.name, base) end
				end
			end
		end
	end
end

------------------------------------------------------------------------------------------------------------------------------------------------------

-- BACKBONE TECHNOLOGIES - different to minimals in that they consult material-component tables - defined in globals

for techname,techdata in pairs(DIR.technologies) do
	local ingredients = {}
	for _,pack in pairs(techdata.packs) do
		table.insert(ingredients, { DIR.science_packs[pack], 1 })
	end
	local icon = nil
	local icons = nil
	if not techdata.icon and not techdata.icons then
		error("Native tech "..techname.." does not provide icon or icons")
	elseif not techdata.icons then
		icon = get_icon_path(techdata.icon,128)
	else 
		icons = {}
		local i = table_length(techdata.icons) - 1
		local s = 1/3
		local dy = 42
		local dx = -42
		local ddx = 42
		local count = 1
		for _,v in pairs(techdata.icons) do
			if count == 1 then
				table.insert(icons, { icon = get_icon_path(v,128), icon_size = 128 })
			else
				table.insert(icons, { icon = get_icon_path(v,128), icon_size = 128, scale = s, shift = { dx, dy } })
				dx = dx + ddx
			end
			count = count + 1
		end
	end
	local tech = {
		name = techname,
		type = "technology",
		icon = icon,
		icons = icons,
		icon_size = 128,
		upgrade = false,
		enabled = true,
		unit = {
			count = 1, -- overridden later
			time = 60,
			ingredients = ingredients,
		},
		prerequisites = techdata.prerequisites,
		effects = {},
	}
	for _,unlock in pairs(techdata.machines) do
		table.insert(tech.effects, { type = "unlock-recipe", recipe = unlock })
	end
	for material,unlocks in pairs(techdata.unlocks) do
		for _,unlock in pairs(unlocks) do
			if unlock == "derivatives" then
				for component,componentdata in pairs(DIR.components) do
					local recipe = get_item_name(material, component)
					if componentdata.derivative and not (componentdata.made_from_exceptions and componentdata.made_from_exceptions[material]) then
						if data.raw.recipe[recipe] then
							table.insert(tech.effects, {type = "unlock-recipe", recipe = recipe })
							if componentdata.reversed then table.insert(tech.effects, {type = "unlock-recipe", recipe = recipe.."-reversed" }) end
						end
					end
				end
			else
				local recipe = get_item_name(material, unlock)
				if data.raw.recipe[recipe] then
					table.insert(tech.effects, { type = "unlock-recipe", recipe = recipe })
					if data.raw.recipe[recipe.."-reversed"] then table.insert(tech.effects, {type = "unlock-recipe", recipe = recipe.."-reversed" }) end
				end
			end
		end
	end
	if techdata.additional_unlocks then
		for _,unlock in pairs(techdata.additional_unlocks) do
			table.insert(tech.effects, { type = "unlock-recipe", recipe = unlock })
		end
	end
	if techdata.category_unlocks then
		for _,recipe in pairs(data.raw.recipe) do
			if recipe.category == techdata.category_unlocks then table.insert(tech.effects, { type = "unlock-recipe", recipe = recipe.name }) end
		end
	end
	data:extend({tech})
end

------------------------------------------------------------------------------------------------------------------------------------------------------

-- TECHNOLOGY CLASSES
-- techs further down the tree inherit dependencies on science packs automatically
-- this partly comes from the DIR tech backbone, but there are special cases that need "seeding" - all defined in globals

for pack,tech_class in pairs(DIR.tech_classes) do
	for _,tech in pairs(tech_class) do
		add_pack_to_tech(DIR.science_packs[pack], tech)
	end
end

-- add space pack to all infinite researches

for _,tech in pairs(data.raw.technology) do
	if tech.enabled ~= false and tech.max_level == "infinite" then
		add_pack_to_tech(DIR.science_packs["space"], tech.name)
	end
end

------------------------------------------------------------------------------------------------------------------------------------------------------

-- SCIENCE PACK PREREQUISITES
-- any tech that uses a science pack must have that science pack unlock as a prereq
-- any redundancies are stripped later on

for _,tech in pairs(data.raw.technology) do
	if tech.enabled ~= false and tech.effects then
		-- add prereqs for packs
		for id,pack in pairs(DIR.science_packs) do
			if id ~= "automation" then
				for _,ingredient in pairs(tech.unit.ingredients) do
					if ingredient[1] == pack then
						if not tech.prerequisites then tech.prerequisites = {} end
						table.insert(tech.prerequisites, pack)
					end
				end
			end
		end
	end
end

------------------------------------------------------------------------------------------------------------------------------------------------------