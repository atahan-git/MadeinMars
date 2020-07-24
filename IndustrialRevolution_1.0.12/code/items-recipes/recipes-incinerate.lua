------------------------------------------------------------------------------------------------------------------------------------------------------

-- INCINERATION AND VOIDING

-- make a recipe for every existing item, fluid, module, equipment, ammo, tool; with no result

------------------------------------------------------------------------------------------------------------------------------------------------------

local function incinerate(name, energy)
	data:extend({{
		type = "recipe",
		name = "incinerate-"..name,
		localised_name = {"recipe-name.incinerating"},
		category = "incinerating",
		results = {},
		icon = get_icon_path("deprogram"),
		icon_size = DIR.icon_size,
		subgroup = "other",
		ingredients = { {name, 1} },
		hidden = true,
		energy_required = energy,
		allow_decomposition = false,
		hide_from_stats = true,
		enabled = true,
	}})
end

local function voiding(name, energy)
	data:extend({{
		type = "recipe",
		name = "void-"..name,
		localised_name = {"recipe-name.voiding"},
		category = "voiding",
		results = {},
		icon = get_icon_path("deprogram"),
		icon_size = DIR.icon_size,
		subgroup = "other",
		ingredients = { {type = "fluid", name = name, amount = 1} },
		hidden = true,
		energy_required = 6/40, -- scaled to basic oil processing output
		allow_decomposition = false,
		hide_from_stats = true,
		enabled = true,
	}})
end


------------------------------------------------------------------------------------------------------------------------------------------------------

for _,type in pairs({"item","module","tool","repair-tool","ammo","capsule","gun","rail-planner","armor","item-with-entity-data","blueprint","blueprint-book","upgrade-item","deconstruction-item"}) do
	for _,item in pairs(data.raw[type]) do
		local energy = data.raw.recipe[item] and (data.raw.recipe[item].energy_required / 2) or 0.5
		if not item.fuel_value then incinerate(item.name, energy) end
	end
end

for _,name in pairs({"crude-oil","heavy-oil","light-oil","petroleum-gas"}) do
	voiding(name)
end

------------------------------------------------------------------------------------------------------------------------------------------------------
