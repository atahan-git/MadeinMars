------------------------------------------------------------------------------------------------------------------------------------------------------

-- SCRAPPING

require("code.data.globals")
require("code.functions.functions-scrapping")
require("code.functions.functions-maths")
require("code.functions.functions-icons")
require("code.functions.functions-entities")

------------------------------------------------------------------------------------------------------------------------------------------------------

local scraptable = {}

-- IR ingots to calculate scrap values for
local scraplist = {"iron","copper","tin","gold","bronze","steel","lead","glass","titanium","duranium"}

-- Krastorio tantalum support
if data.raw.item["tantalum-ingot"] and data.raw.item["tantalum-scrap"] then table.insert(scraplist,"tantalum") end

-- generate scrap table
for _,material in pairs(scraplist) do
    calculate_scraps({item = material.."-ingot", base_value = 1 / DIR.scrap_divider, base_item = material.."-scrap"}, scraptable)
end

-- manually do stone - produces gravel
calculate_scraps({item = "stone", base_value = 1, base_item = "stone"}, scraptable)
calculate_scraps({item = "stone-brick", base_value = 2, base_item = "stone"}, scraptable)
calculate_scraps({item = "stone-gravel", base_value = 1, base_item = "stone-gravel"}, scraptable)

-- thin scraps down to maximum of 5
for name,scrapdata in pairs(scraptable) do
	while table_size(scrapdata) > 5 do
		local lowest = nil
		for scrap,amount in pairs(scrapdata) do
			if lowest == nil or (scrapdata[lowest] > amount) then lowest = scrap end
		end
		scrapdata[lowest] = nil
		spam_log("Removed "..lowest.." from "..name)
	end
end

-- generate recipes
for name,scrapdata in pairs(scraptable) do
	-- results
    local results = {}
    local description = {"", {"item-description.scrap"}, " ", {"recipe-description.font-wrap-open"}}
    local so_called_energy = 0
	local count = 1
    for scrap,amount in pairs(scrapdata) do
		-- if count == 4 then table.insert(description,"\n") end
        so_called_energy = so_called_energy + amount
		-- cap scrap at 50k because of other mods with insane recipes
		local maximum = math.min(50000, math.floor(amount))
		local minimum = math.min(50000, math.max(1, math.floor(amount * DIR.scrap_minimum_rate)))
		local probability = amount * ((DIR.scrap_minimum_rate+1)/2)
        if probability >= 1 then
			local average = (minimum+maximum)/2
			if average >= 1000 then average = string.format("%.1f", math.floor(average/100)/10) .. "k"
			elseif average >= 10 then average = string.format("%d", average)
			else average = string.format("%s", math.floor(average*10)/10) end
            table.insert(results, {name = scrap, type = "item", amount_max = maximum, amount_min = minimum})
			table.insert(description, string.format("[img=item/%s] %s ", scrap, average))
        else
            table.insert(results, {name = scrap, type = "item", amount = 1, probability = probability})
			table.insert(description, string.format("[img=item/%s] %.1f ", scrap, probability))
        end
		count = count + 1
    end
	table.insert(description, {"recipe-description.font-wrap-close"})
	so_called_energy = math.max(0.1, math.floor(math.sqrt(so_called_energy)*10)/10)
	-- recipe
    data:extend({{
        name = "scrap-"..name,
		localised_name = {"recipe-name.scrapping"},
        type = "recipe",
        category = "scrapping",
        subgroup = "scrapping",
        hidden = true,
        allow_decomposition = false,
        allow_intermediates = false,
        allow_as_intermediate = false,
        icon = get_icon_path("disassemble"),
		icon_size = DIR.icon_size,
        enabled = true,
        ingredients = { { name = name, type = "item", amount = 1 } },
        results = results,
        energy_required = so_called_energy,
    }})
end

-- look for recipes without a crafting tint and use most common material's standard tint
-- also add icon overlay to recipe
for name,scrapdata in pairs(scraptable) do
	local biggest_value = 0
	local biggest_material = nil
	local biggest_scrap = nil
	if data.raw.recipe[name] and data.raw.recipe[name].crafting_machine_tint == nil then
		for scrap,amount in pairs(scrapdata) do
			if amount > biggest_value then
				biggest_value = amount
				biggest_scrap = scrap
				biggest_material = string.match(scrap,"%a+")
			end
		end
		if biggest_material ~= nil and DIR.materials[biggest_material] then 
			data.raw.recipe[name].crafting_machine_tint = get_crafting_tints_from_hue(DIR.materials[biggest_material].hue,DIR.materials[biggest_material].saturation)
		end
	end
	if biggest_scrap ~= nil and data.raw.item[biggest_scrap] ~= nil and data.raw.recipe["scrap-"..name] then
		convert_recipe_icon_to_icons(data.raw.recipe["scrap-"..name])
		table.insert(data.raw.recipe["scrap-"..name].icons, {icon = data.raw.item[biggest_scrap].icon, icon_size = DIR.icon_size, scale = 0.375})
	end
end

-- add scrap loot to entities
if settings.startup["deadlock-industry-building-scrap"].value then
	for name,scrapdata in pairs(scraptable) do
		local entity = wild_stab_at_entity_by_name(data.raw.item[name] and data.raw.item[name].place_result)
		if entity then
			-- set up loot table
			if entity.loot == nil then entity.loot = {} end
			local total = 0
			for scrap,amount in pairs(scrapdata) do
				total = total + amount
				if amount * DIR.scrap_minimum_rate * DIR.scrap_loot_penalty >= 1 then
					table.insert(entity.loot, {item = scrap, count_max = math.max(1, math.floor(amount * DIR.scrap_loot_penalty)), count_min = math.floor(amount * DIR.scrap_minimum_rate * DIR.scrap_loot_penalty)})
				else
					table.insert(entity.loot, {item = scrap, count_max = 1, count_min = 1, probability = amount * ((DIR.scrap_minimum_rate+1)/2) * DIR.scrap_loot_penalty})
				end
			end
			-- 9 items can fit into a tile. If the total loot was more than 9 items per 75% of the selection box area, scale everything down in proportion to total
			local area = 9 * get_area_of_box(entity.selection_box) * 0.75
			if total > area then
				for _,lootable in pairs(entity.loot) do
					if lootable.probability then
						lootable.probability = lootable.probability * (area/total)
					else 
						lootable.count_max = math.max(1, math.floor(linear_transform(lootable.count_max,1,total,1,area)))
						lootable.count_min = math.max(1, math.floor(lootable.count_max * DIR.scrap_minimum_rate))
					end
				end
			end
		end
	end
	if data.raw["electric-turret"]["photon-turret"] and data.raw["electric-turret"]["photon-turret-narrow"] and data.raw["electric-turret"]["photon-turret"].loot then
		data.raw["electric-turret"]["photon-turret-narrow"].loot = data.raw["electric-turret"]["photon-turret"].loot
	end
end

------------------------------------------------------------------------------------------------------------------------------------------------------
