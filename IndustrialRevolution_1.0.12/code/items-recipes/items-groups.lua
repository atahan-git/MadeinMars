------------------------------------------------------------------------------------------------------------------------------------------------------

require("code.functions.functions-icons")

------------------------------------------------------------------------------------------------------------------------------------------------------

-- tidy up that messy intermediates group a bit

data.raw["item-group"]["intermediate-products"].icon = get_icon_path("intermediate-products", 128)
data.raw["item-group"]["intermediate-products"].icon_size = 128
data.raw["item-subgroup"]["raw-resource"].order = "b"
data.raw["item-subgroup"]["raw-material"].order = "c"
data.raw["item-subgroup"]["intermediate-product"].order = "d"
data.raw["item-subgroup"]["science-pack"].order = "g"
data.raw["item-subgroup"]["fluid-recipes"].order = "zzz" 
data.raw["item-subgroup"]["fluid-recipes"].group = "deadlock-processing" 
data.raw["item-subgroup"]["fill-barrel"].order = "h"
data.raw["item-subgroup"]["empty-barrel"].order = "i"

data.raw["item-group"]["logistics"].icon = "__base__/graphics/technology/logistics.png"
data.raw["item-group"]["logistics"].icon_size = 128
data.raw["item-group"]["production"].icon = get_icon_path("production",128)
data.raw["item-group"]["production"].icon_size = 128
data.raw["item-group"]["combat"].icon = "__base__/graphics/technology/military.png"
data.raw["item-group"]["combat"].icon_size = 128
data.raw["item-group"]["fluids"].icon = get_icon_path("fluids",128)
data.raw["item-group"]["fluids"].icon_size = 128
data.raw["item-group"]["signals"].icon = get_icon_path("signals",128)
data.raw["item-group"]["signals"].icon_size = 128

------------------------------------------------------------------------------------------------------------------------------------------------------

-- DIR basics: crafting categories

for _,category in pairs(DIR.crafting_categories) do
    data:extend({
        {
            type = "recipe-category",
            name = category,
        }
    })
end

for _,category in pairs(DIR.tiered_categories) do
	for i=1,DIR.max_tiers do
		data:extend({
			{
				type = "recipe-category",
				name = category.."-"..i,
			}
		})
	end
end

------------------------------------------------------------------------------------------------------------------------------------------------------

-- item groups, subgroups

data:extend({
    {
        type = "item-group",
        name = "deadlock-processing",
        order = "bx",
        icon = get_icon_path("processing", 128),
        icon_size = 128,
    },
    {
        type = "item-group",
        name = "deadlock-basics",
        order = "by",
        icon = get_icon_path("basic-products", 128),
        icon_size = 128,
    },
	{
		type = "item-group",
		name = "deadlock-scrapping",
		order = "z",
		icon = get_icon_path("scrapping", 128),
		icon_size = 128,
	},
    {
        type = "item-subgroup",
        name = "wood",
        group = "deadlock-processing",
        order = "zzzzzzz",
    },
    {
        type = "item-subgroup",
        name = "non-handcrafting",
        group = "deadlock-processing",
        order = "z",
    },
    {
        type = "item-subgroup",
        name = "scrapping",
        group = "deadlock-scrapping",
        order = "a",
    },
	{
		type = "item-subgroup",
		name = "deadlock-electronics",
		group = "intermediate-products",
		order = "aa",
	},
	{
		type = "item-subgroup",
		name = "deadlock-robots",
		group = "logistics",
		order = "eb",
	},
	{
		type = "item-subgroup",
		name = "deadlock-module-reversed",
		group = "production",
		order = "y",
	},
	{
		type = "item-subgroup",
		name = "deadlock-filters",
		group = "production",
		order = "z",
	},
	{
		type = "item-subgroup",
		name = "virtual-signal-map-markers",
		group = "signals",
		order = "zzzzz",
	},
	{
		type = "item-subgroup",
		name = "ingots-1",
		group = "deadlock-processing",
		order = "e1",
	},
	{
		type = "item-subgroup",
		name = "ingots-2",
		group = "deadlock-processing",
		order = "e2",
	},
	{
		type = "item-subgroup",
		name = "fluid-recipes-2",
		group = "deadlock-processing",
		order = "zzz2",
	},
	{
		type = "item-subgroup",
		name = "uranium-processing",
		group = "deadlock-processing",
		order = "xxx",
	},
	{
		type = "item-subgroup",
		name = "space-products",
		group = "intermediate-products",
		order = "f",
	},
	{
		type = "item-subgroup",
		name = "deadlock-walls",
		group = "combat",
		order = "zzz",
	},
})

-- basic product subgroups

for component,componentdata in pairs(DIR.components) do
	local tabgroup = componentdata.tabgroup or "deadlock-basics"
	local subgroup = componentdata.subgroup or component
	if not data.raw["item-subgroup"][subgroup] then 
		data:extend({{
			type = "item-subgroup",
			name = subgroup,
			group = tabgroup,
			order = componentdata.order,
		}})
	end
	if componentdata.item_subgroup and not data.raw["item-subgroup"][componentdata.item_subgroup] then
		data:extend({{
			type = "item-subgroup",
			name = componentdata.item_subgroup,
			group = tabgroup,
			order = componentdata.order,
		}})
	end
end

-- machine subgroups

local count = 1
for _,machinedata in pairs(DIR.machines) do
	if not data.raw["item-subgroup"][machinedata.subgroup] then
		data:extend {{
			type = "item-subgroup",
			name = machinedata.subgroup,
			order = "a"..count,
			group = "production",
		}}
		count = count + 1
	end
end

------------------------------------------------------------------------------------------------------------------------------------------------------
