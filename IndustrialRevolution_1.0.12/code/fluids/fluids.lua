------------------------------------------------------------------------------------------------------------------------------------------------------

require("code.functions.functions-recipes")

------------------------------------------------------------------------------------------------------------------------------------------------------

data:extend({
	{
		type = "fluid",
		name = "dirty-water",
		base_color = {
			r = 0.4,
			g = 0.2,
			b = 0.0,
		},
		default_temperature = 15,
		flow_color = {
			r = 0.5,
			g = 0.5,
			b = 0.5,
		},
		heat_capacity = "0.2KJ",
		icon = get_icon_path("dirty-water", DIR.icon_size),
		icon_size = DIR.icon_size,
		max_temperature = 100,
		order = "zzz",
		subgroup = "fluid",
		auto_barrel = true,
	},
	{
		type = "recipe",
		name = "dirty-water-cleaning",
		order = "zzz",
		subgroup = "fluid-recipes-2",
		ingredients = {
			{name = "dirty-water", type = "fluid", amount = DIR.washing_base * DIR.washers_to_cleaners, catalyst_amount = DIR.washing_base * DIR.washers_to_cleaners},
		},
		results = { 
			{name = "water", type = "fluid", amount = DIR.washing_base * DIR.washers_to_cleaners * DIR.washing_ratio, catalyst_amount = DIR.washing_base * DIR.washers_to_cleaners * DIR.washing_ratio},
			{name = "stone-gravel", type = "item", amount = 1, probability = 0.2},
			{name = "sand", type = "item", amount = 1, probability = 0.1},
		},
		icon = get_icon_path("water", DIR.icon_size),
		icon_size = DIR.icon_size,
		category = "cleaning",
		subgroup = "fluid-recipes-2",
		enabled = false,
		energy_required = DIR.components["pure"].speed,
		allow_decomposition = false,
	}
})

data:extend({
	{
		type = "fluid",
		name = "diluted-sulfuric-acid",
		base_color = brighter_colour(data.raw.fluid["sulfuric-acid"].base_color),
		flow_color = brighter_colour(data.raw.fluid["sulfuric-acid"].flow_color),
		default_temperature = 25,
		heat_capacity = "0.2KJ",
		icon = get_icon_path("diluted-sulfuric-acid", DIR.icon_size),
		icon_size = DIR.icon_size,
		max_temperature = 100,
		order = "a[fluid]-f[z]",
		subgroup = "fluid",
		auto_barrel = false,
	},
	{
		type = "recipe",
		name = "diluted-sulfuric-acid",
		order = "aab",
		subgroup = "fluid-recipes-2",
		ingredients = {
			{name = "water", type = "fluid", amount = DIR.washing_base * 5, catalyst_amount = DIR.washing_base * 5},
			{name = "sulfuric-acid", type = "fluid", amount = DIR.washing_base, catalyst_amount = DIR.washing_base},
		},
		results = { 
			{name = "diluted-sulfuric-acid", type = "fluid", amount = DIR.washing_base * 6, catalyst_amount = DIR.washing_base * 6},
		},
		icon = get_icon_path("diluted-sulfuric-acid", DIR.icon_size),
		icon_size = DIR.icon_size,
		category = "chemistry",
		subgroup = "fluid-recipes-2",
		enabled = false,
		energy_required = DIR.components["pure"].speed,
        crafting_machine_tint = data.raw.recipe["sulfuric-acid"].crafting_machine_tint,
        show_amount_in_title = false,
        always_show_products = true,
	}
})

add_result_icons_to_recipe(data.raw.recipe["dirty-water-cleaning"],"water")

------------------------------------------------------------------------------------------------------------------------------------------------------
