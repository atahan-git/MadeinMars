------------------------------------------------------------------------------------------------------------------------------------------------------

-- TERRAIN - ores etc.

local function resource(name, map_color, base_density, has_starting_area_placement, base_spots, spot_min, spot_max)
    return {
        type = "resource",
        name = name,
        icon = get_icon_path(name, DIR.icon_size),
        icon_size = DIR.icon_size,
        flags = {"placeable-neutral"},
        order = "a",
        minable =
        {
            mining_time = 1,
			mining_particle = "stone-particle",
            result = name
        },
        collision_box = {{ -0.1, -0.1}, {0.1, 0.1}},
        selection_box = {{ -0.5, -0.5}, {0.5, 0.5}},
        autoplace = resource_autoplace.resource_autoplace_settings {
			name = name,
			order = "b",
			base_density = base_density,
			has_starting_area_placement = has_starting_area_placement,
			regular_rq_factor_multiplier = 1.1,
			starting_rq_factor_multiplier = 1.2,
			candidate_spot_count = 22,
			base_spots_per_km2 = base_spots,
			random_spot_size_minimum = spot_min,
			random_spot_size_maximum = spot_max,
		},
		stage_counts = {15000,9500,5500,2900,1300,400,150,80},
		stages = {
            sheet =
            {
                filename = DIR.terrain_path .. "/" .. name .. ".png",
                priority = "extra-high",
                width = 64,
                height = 64,
                frame_count = 8,
                variation_count = 8,
                hr_version = {
                    filename = DIR.terrain_path .. "/hr-" .. name .. ".png",
                    priority = "extra-high",
                    width = 128,
                    height = 128,
                    frame_count = 8,
                    variation_count = 8,
                    scale = 0.5
                }
            }
        },
		tree_removal_max_distance = 1024,
		tree_removal_probability = 0.9,
        map_color = map_color
    }
end

-- remove iron from starting area

data.raw.resource["iron-ore"].autoplace = resource_autoplace.resource_autoplace_settings {
	name = "iron-ore",
	order = "b",
	base_density = 10,
	has_starting_area_placement = false,
	regular_rq_factor_multiplier = 1.1,
	candidate_spot_count = 22,
}

-- add tin, gold

data:extend { resource("tin-ore", {r=0.3, g=0.3, b=0.5}, 7, true) }
data:extend { resource("gold-ore", {r=0.9, g=0.8, b=0.1}, 1, false, 1.25, 2, 4) }

data.raw.resource["gold-ore"].minable.required_fluid = "water"
data.raw.resource["gold-ore"].minable.fluid_amount = 20

data:extend {{ type = "noise-layer", name = "tin-ore" }}
data:extend {{ type = "noise-layer", name = "gold-ore" }}

data:extend {{
        type = "autoplace-control",
        name = "tin-ore",
        richness = true,
        order = "b-ab",
        category = "resource"
}}    
data:extend {{    
        type = "autoplace-control",
        name = "gold-ore",
        richness = true,
        order = "b-bb",
        category = "resource"
}}

------------------------------------------------------------------------------------------------------------------------------------------------------
