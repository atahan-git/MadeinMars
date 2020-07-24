------------------------------------------------------------------------------------------------------------------------------------------------------

-- TREES

------------------------------------------------------------------------------------------------------------------------------------------------------

-- add saplings to all vanilla trees

local sapling = {type = "item", name = "wood-sapling", amount = 1, probability = 0.1}

for _,tree in pairs(data.raw.tree) do
    if not string.find(tree.name, "dead") and not string.find(tree.name, "dry") then
		-- did some other mod remove all results?
		if not tree.minable then
			tree.minable = { mining_time = 0.5 }
		end
		if not tree.minable.results then
			-- does the tree produce a single result?
			if tree.minable.result then 
				-- there's a single result - add a sapling
				tree.minable.results = {
					{type = "item", name = tree.minable.result, amount = tree.minable.count or 1},
					sapling,
				}
				tree.minable.result = nil
				tree.minable.count = nil
			else
				-- there's no result for whatever reason - put it back again
				tree.minable.results = {
					{type = "item", name = "wood", amount = 4},
					sapling,
				}
				tree.minable.result = nil
				tree.minable.count = nil
			end
		else
			-- if another mod already replaced with multiple results, just add sapling
			table.insert(tree.minable.results, sapling)
		end
    end
end

-- rubber tree

data:extend({
	{
		name = "deadlock-rubber-trees",
		type = "autoplace-control",
		category = "terrain",
		order = "c-z",
		richness = true,
		localised_description = {"autoplace-control-descriptions.deadlock-rubber-trees"},
	},
	{
		name = "deadlock-rubber-trees-1",
		type = "noise-layer",
	},
	{
		name = "deadlock-rubber-trees-2",
		type = "noise-layer",
	},
})

local tree = table.deepcopy(data.raw.tree["tree-09"])
local tree_colour = {0.5,0.25,0.05,1.0}
tree.name = "deadlock-rubber-tree"
tree.map_color = {r=60,g=90,b=30,a=255}
tree.control = "deadlock-rubber-trees"
tree.icon = get_icon_path("rubber-tree",DIR.icon_size)
tree.icon_size = DIR.icon_size
tree.colors = {
    {r=1.0,g=1.0,b=0.0},
    {r=0.9,g=0.9,b=0.0},
    {r=0.8,g=0.8,b=0.0},
    {r=0.7,g=0.7,b=0.0},
    {r=0.6,g=0.6,b=0.0},
    {r=0.5,g=0.5,b=0.0},
}
tree.minable.results = {
    {type = "item", name = "rubber-wood", amount = 4},
    {type = "item", name = "rubber-sapling", amount = 1, probability = 0.2},
}
tree.autoplace = {
    control = "deadlock-rubber-trees",
    max_probability = 0.1,
    order = "a[tree]-b[forest]",
    peaks = {
        {
            influence = 0,
            richness_influence = 0.75
        },
        {
            influence = -0.75,
            richness_influence = 0.75
        },
        {
            influence = 0.375,
            noise_layer = "deadlock-rubber-trees-1",
            noise_octaves_difference = -0.5,
            noise_persistence = 0.8,
            richness_influence = -0.5
        },
        {
            influence = 0.5,
            noise_layer = "deadlock-rubber-trees-2",
            noise_octaves_difference = -0.5,
            noise_persistence = 0.6,
            richness_influence = -0.5
        },
        {
			min_influence = 0,
			max_influence = 1,
            temperature_max_range = 12.5,
            temperature_optimal = 27.5,
            temperature_range = 7.5,
            water_max_range = 0.3,
            water_optimal = 0.75,
            water_range = 0.25
        },
        {
			min_influence = 0,
			max_influence = 3,
            elevation_max_range = 3,
            elevation_optimal = 0,
            elevation_range = 1,
        },
        {
			min_influence = -0.5,
			max_influence = 0,
            starting_area_weight_max_range = 0.5,
            starting_area_weight_optimal = 0,
            starting_area_weight_range = 0.5,
        },
    },
    random_probability_penalty = 0.001,
    richness_base = 0,
    richness_multiplier = 1,
    sharpness = 0.4
}
for _,variation in pairs(tree.variations) do
    if variation.trunk then
        variation.trunk.tint = tree_colour
        variation.trunk.hr_version.tint = tree_colour
    end
end
data:extend({tree})

------------------------------------------------------------------------------------------------------------------------------------------------------
