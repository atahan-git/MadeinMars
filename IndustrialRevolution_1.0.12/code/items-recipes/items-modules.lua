------------------------------------------------------------------------------------------------------------------------------------------------------

-- rework vanilla modules

local modules = {[1] = "speed", [2] = "effectivity", [3] = "productivity"}
local computers = {[1] = "computer-mk1", [2] = "computer-mk2", [3] = "computer-mk3"}

for i=1,3 do
	for level=1,3 do
		local name = modules[i].."-module"..((level > 1) and "-"..level or "")
		-- hide vanilla recipe
		data.raw.recipe[name].hidden = true
		-- update icon
		replace_item_icon(name)
		-- update limitations
		if modules[i] == "productivity" then
			data.raw.module[name].limitation = DIR.productivity_limitation
		end
		-- create new program/format recipes
		data:extend({
			{
				type = "recipe",
				name = "program-"..name,
				result = name,
				result_count = 1,
				category = "crafting",
				enabled = false,
				ingredients = {
					{computers[level],1},
				},
				energy_required = 2 * (2^(level-1)),
				subgroup = "module",
				hide_from_stats = true,
				always_show_products = true,
				localised_name = {"recipe-name.program-module",{"item-name."..name}},
				order = name,
			},
			{
				type = "recipe",
				name = "deprogram-"..name,
				result = computers[level],
				result_count = 1,
				category = "crafting",
				enabled = false,
				ingredients = {
					{name,1},
				},
				icons = {
					{ icon = get_icon_path(name,DIR.icon_size), icon_size = DIR.icon_size},
					{ icon = get_icon_path("deprogram",DIR.icon_size), icon_size = DIR.icon_size, scale = 0.25, shift = {-8,8}},
				},
				energy_required = 1,
				subgroup = "deadlock-module-reversed",
				hide_from_stats = true,
				allow_as_intermediate = false,
				allow_intermediates = false,
				always_show_products = true,
				localised_name = {"recipe-name.deprogram-module",{"item-name."..name}},
				order = name,
			}
		})
	end
end

------------------------------------------------------------------------------------------------------------------------------------------------------

-- pollution filter

local prefixes = {"simple","standard","advanced"}
local suffix = "-pollution-filter"
local material1 = {"bronze","iron","steel"}
local material2 = {"copper-plate","gold-plate","platinum-pure"}

data:extend({
	{
		name = "pollution-filter",
		type = "module-category",
	}
})

for i = 1,3 do
	local name = prefixes[i]..suffix
	local ingredients = {
		{material1[i].."-chassis-small", 1},
		{material1[i].."-tube", 2},
		{material2[i], (i==3) and 4 or 16},
	}
	data:extend({
		{
			name = name,
			type = "module",
			category = "pollution-filter",
			effect = {
				pollution = {
					bonus = -0.2 * i
				}
			},
			icon = get_icon_path(name, DIR.icon_size),
			icon_size = DIR.icon_size,
			localised_description = {"item-description.filter"},
			order = "a"..i,
			stack_size = 50,
			subgroup = "deadlock-filters",
			tier = i,
		},
		{
			name = prefixes[i]..suffix,
			type = "recipe",
			result = name,
			result_count = 1,
			category = "crafting",
			enabled = false,
			always_show_made_in = true,
			ingredients = ingredients,
			energy_required = i,
		}
	})
end

------------------------------------------------------------------------------------------------------------------------------------------------------
