------------------------------------------------------------------------------------------------------------------------------------------------------

-- oil processing

local cycle = 6
local input_fluid = cycle * 20
local output_fluid = input_fluid / 3
local water_ratio = 0.5
local cracking_emissions = 1.5
local solid_fuel_ratio = 2
local advanced_cracking_time= 1.25
local cracking_ratio = 0.5
local advanced_cracking_ratio = 0.75
local overlay_icon_scale = 0.4

data:extend({
    {
        name = "basic-oil-processing",
        type = "recipe",
        category = "oil-processing",
        enabled = false,
        energy_required = cycle,
        icons = {
            { icon = get_icon_path("cracking-background",DIR.icon_size), icon_size = DIR.icon_size},
            { icon = get_icon_path("crude-oil",DIR.icon_size), icon_size = DIR.icon_size, scale = overlay_icon_scale},
        },
        ingredients = {
            {
                name = "crude-oil",
                amount = input_fluid,
                type = "fluid"
            },
            {
                name = "water",
                amount = input_fluid * water_ratio,
                type = "fluid"
            },
        },
        results = {
            {
                name = "heavy-oil",
                amount = output_fluid,
                type = "fluid"
            },
            {
                name = "light-oil",
                amount = output_fluid,
                type = "fluid"
            },
            {
                name = "petroleum-gas",
                amount = output_fluid,
                type = "fluid"
            },
        },
        main_product = "",
        subgroup = "fluid-recipes",
        order = "a",
    },
    {
        name = "solid-fuel-from-heavy-oil",
        type = "recipe",
        category = "chemistry",
        enabled = false,
        energy_required = cycle / solid_fuel_ratio,
        icons = {
            { icon = get_icon_path("cracking-background",DIR.icon_size), icon_size = DIR.icon_size},
            { icon = get_icon_path("solid-fuel-red",DIR.icon_size), icon_size = DIR.icon_size, scale = overlay_icon_scale},
        },
        crafting_machine_tint = data.raw.recipe["solid-fuel-from-heavy-oil"].crafting_machine_tint,
        ingredients = {
            {
                name = "water",
                amount = output_fluid * water_ratio / solid_fuel_ratio,
                type = "fluid"
            },
            {
                name = "heavy-oil",
                amount = output_fluid / solid_fuel_ratio,
                type = "fluid"
            },
        },
        results = {
            {
                name = "solid-fuel-red",
                amount = 1,
                type = "item"
            },
        },
        allow_decomposition = false,
        show_amount_in_title = false,
        always_show_products = true,
        order = "b",
    },
    {
        name = "solid-fuel-from-light-oil",
        type = "recipe",
        category = "chemistry",
        enabled = false,
        energy_required = cycle / solid_fuel_ratio,
        icons = {
            { icon = get_icon_path("cracking-background",DIR.icon_size), icon_size = DIR.icon_size},
            { icon = get_icon_path("solid-fuel-yellow",DIR.icon_size), icon_size = DIR.icon_size, scale = overlay_icon_scale},
        },
        crafting_machine_tint = data.raw.recipe["solid-fuel-from-light-oil"].crafting_machine_tint,
        ingredients = {
            {
                name = "water",
                amount = output_fluid * water_ratio / solid_fuel_ratio,
                type = "fluid"
            },
            {
                name = "light-oil",
                amount = output_fluid / solid_fuel_ratio,
                type = "fluid"
            },
        },
        results = {
            {
                name = "solid-fuel-yellow",
                amount = 1,
                type = "item"
            },
        },
        allow_decomposition = false,
        show_amount_in_title = false,
        always_show_products = true,
        subgroup = "fluid-recipes",
        order = "c",
    },
    {
        name = "solid-fuel-from-petroleum-gas",
        type = "recipe",
        category = "chemistry",
        enabled = false,
        energy_required = cycle / solid_fuel_ratio,
        icons = {
            { icon = get_icon_path("cracking-background",DIR.icon_size), icon_size = DIR.icon_size},
            { icon = get_icon_path("solid-fuel",DIR.icon_size), icon_size = DIR.icon_size, scale = overlay_icon_scale},
        },
        crafting_machine_tint = data.raw.recipe["solid-fuel-from-petroleum-gas"].crafting_machine_tint,
        ingredients = {
            {
                name = "water",
                amount = output_fluid * water_ratio / solid_fuel_ratio,
                type = "fluid"
            },
            {
                name = "petroleum-gas",
                amount = output_fluid / solid_fuel_ratio,
                type = "fluid"
            },
        },
        results = {
            {
                name = "solid-fuel",
                amount = 1,
                type = "item"
            },
        },
        allow_decomposition = false,
        show_amount_in_title = false,
        always_show_products = true,
        subgroup = "fluid-recipes",
        order = "d",
    },
    {
        name = "heavy-oil-cracking",
        type = "recipe",
        category = "chemistry",
        enabled = false,
        energy_required = cycle,
        icons = {
            { icon = get_icon_path("cracking-background",DIR.icon_size), icon_size = DIR.icon_size},
            { icon = get_icon_path("heavy-oil",DIR.icon_size), icon_size = DIR.icon_size, scale = overlay_icon_scale},
        },
        crafting_machine_tint = data.raw.recipe["heavy-oil-cracking"].crafting_machine_tint,
        ingredients = {
            {
                name = "water",
                amount = output_fluid * water_ratio,
                type = "fluid"
            },
            {
                name = "heavy-oil",
                amount = output_fluid,
                type = "fluid"
            },
        },
        results = {
            {
                name = "light-oil",
                amount = output_fluid * cracking_ratio,
                type = "fluid"
            },
        },
        emissions_multiplier = cracking_emissions,
        allow_decomposition = false,
        main_product = "",
        subgroup = "fluid-recipes",
        order = "e",
    },
    {
        name = "light-oil-cracking",
        type = "recipe",
        category = "chemistry",
        enabled = false,
        energy_required = cycle,
        icons = {
            { icon = get_icon_path("cracking-background",DIR.icon_size), icon_size = DIR.icon_size},
            { icon = get_icon_path("light-oil",DIR.icon_size), icon_size = DIR.icon_size, scale = overlay_icon_scale},
        },
        crafting_machine_tint = data.raw.recipe["light-oil-cracking"].crafting_machine_tint,
        ingredients = {
            {
                name = "water",
                amount = output_fluid * water_ratio,
                type = "fluid"
            },
            {
                name = "light-oil",
                amount = output_fluid,
                type = "fluid"
            },
        },
        results = {
            {
                name = "petroleum-gas",
                amount = output_fluid * cracking_ratio,
                type = "fluid"
            },
        },
        emissions_multiplier = cracking_emissions,
        allow_decomposition = false,
        main_product = "",
        subgroup = "fluid-recipes",
        order = "f",
    },
    {
        name = "lubricant",
        type = "recipe",
        category = "chemistry",
        enabled = false,
        energy_required = cycle / 2,
        icons = {
            { icon = get_icon_path("cracking-background",DIR.icon_size), icon_size = DIR.icon_size},
            { icon = get_icon_path("lubricant",DIR.icon_size), icon_size = DIR.icon_size, scale = overlay_icon_scale},
        },
        crafting_machine_tint = data.raw.recipe["lubricant"].crafting_machine_tint,
        ingredients = {
            {
                name = "heavy-oil",
                amount = output_fluid / 2,
                type = "fluid"
            },
        },
        results = {
            {
                name = "lubricant",
                amount = output_fluid / 2,
                type = "fluid"
            },
        },
        allow_decomposition = false,
        show_amount_in_title = false,
        always_show_products = true,
        subgroup = "fluid-recipes",
        order = "g",
    },
    {
        name = "advanced-heavy-oil-cracking",
        type = "recipe",
        category = "chemistry",
        enabled = false,
        energy_required = cycle * advanced_cracking_time,
        icons = {
            { icon = get_icon_path("advanced-cracking-background",DIR.icon_size), icon_size = DIR.icon_size},
            { icon = get_icon_path("heavy-oil",DIR.icon_size), icon_size = DIR.icon_size, scale = overlay_icon_scale},
        },
        crafting_machine_tint = data.raw.recipe["heavy-oil-cracking"].crafting_machine_tint,
        ingredients = {
            {
                name = "water",
                amount = output_fluid * water_ratio,
                type = "fluid"
            },
            {
                name = "heavy-oil",
                amount = output_fluid,
                type = "fluid"
            },
            {
                name = "platinum-pure",
                amount = 1,
                type = "item",
                catalyst_amount = 1,
            },
        },
        results = {
            {
                name = "light-oil",
                amount = output_fluid * advanced_cracking_ratio,
                type = "fluid"
            },
            {
                name = "platinum-pure",
                amount = 1,
                probability = 0.99,
                type = "item",
                catalyst_amount = 1,
            },
        },
        allow_decomposition = false,
        main_product = "",
        subgroup = "fluid-recipes",
        order = "h",
    },
    {
        name = "advanced-light-oil-cracking",
        type = "recipe",
        category = "chemistry",
        enabled = false,
        energy_required = cycle * advanced_cracking_time,
        icons = {
            { icon = get_icon_path("advanced-cracking-background",DIR.icon_size), icon_size = DIR.icon_size},
            { icon = get_icon_path("light-oil",DIR.icon_size), icon_size = DIR.icon_size, scale = overlay_icon_scale},
        },
        crafting_machine_tint = data.raw.recipe["light-oil-cracking"].crafting_machine_tint,
        ingredients = {
            {
                name = "water",
                amount = output_fluid * water_ratio,
                type = "fluid"
            },
            {
                name = "light-oil",
                amount = output_fluid,
                type = "fluid"
            },
            {
                name = "platinum-pure",
                amount = 1,
                type = "item",
                catalyst_amount = 1,
            },
        },
        results = {
            {
                name = "petroleum-gas",
                amount = output_fluid * advanced_cracking_ratio,
                type = "fluid"
            },
            {
                name = "platinum-pure",
                amount = 1,
                probability = 0.99,
                type = "item",
                catalyst_amount = 1,
            },
        },
        allow_decomposition = false,
        main_product = "",
        subgroup = "fluid-recipes",
        order = "i",
    },
    {
        name = "coal-liquefaction",
        type = "recipe",
        category = "chemistry",
        enabled = false,
        energy_required = cycle,
        icons = {
            { icon = get_icon_path("advanced-cracking-background",DIR.icon_size), icon_size = DIR.icon_size},
            { icon = get_icon_path("coal",DIR.icon_size), icon_size = DIR.icon_size, scale = overlay_icon_scale},
        },
        icon_size = DIR.icon_size,
        crafting_machine_tint = data.raw.recipe["coal-liquefaction"].crafting_machine_tint,
        ingredients = {
            {
                name = "heavy-oil",
                amount = 30,
                type = "fluid"
            },
            {
                name = "steam",
                amount = 20,
                type = "fluid"
            },
            {
                name = "coal",
                amount = 15,
                type = "item",
            },
        },
        results = {
            {
                name = "heavy-oil",
                amount = 80,
                type = "fluid"
            },
            {
                name = "light-oil",
                amount = 40,
                type = "fluid"
            },
        },
        allow_decomposition = false,
        main_product = "",
        subgroup = "fluid-recipes",
        order = "j",
    },
	{
      name = "sulfur",
      type = "item",
      icon = get_icon_path("sulfur"),
      icon_size = DIR.icon_size,
      order = "a",
      stack_size = 100,
      subgroup = "fluid-recipes",
      pictures = {
        {
          filename = get_icon_path("sulfur"),
          scale = 0.25,
          size = 64
        },
        {
          filename = get_icon_path("sulfur-2"),
          scale = 0.25,
          size = 64
        },
        {
          filename = get_icon_path("sulfur-3"),
          scale = 0.25,
          size = 64
        },
        {
          filename = get_icon_path("sulfur-4"),
          scale = 0.25,
          size = 64
        }
      },
	},
    {
        name = "sulfur",
        type = "recipe",
        category = "chemistry",
        crafting_machine_tint = data.raw.recipe["sulfur"].crafting_machine_tint,
        enabled = false,
        energy_required = DIR.machines_to_belt_ratio/2,
        ingredients = {
            {
                amount = output_fluid,
                name = "water",
                type = "fluid"
            },
            {
                amount = output_fluid,
                name = "petroleum-gas",
                type = "fluid"
            },
        },
        results = {
            {
                amount = 3,
                name = "sulfur",
                type = "item"
            }
        },
        show_amount_in_title = false,
        always_show_products = true,
        subgroup = "fluid-recipes-2",
        order = "a",
    },
    {
        name = "sulfuric-acid",
        localised_name = {"recipe-name.sulfuric-acid"},
        type = "recipe",
        category = "chemistry",
        subgroup = "fluid-recipes-2",
        order = "aa",
        crafting_machine_tint = data.raw.recipe["sulfuric-acid"].crafting_machine_tint,
        enabled = false,
        energy_required = DIR.machines_to_belt_ratio/2,
        ingredients = {
            {
                amount = 3,
                name = "sulfur",
                type = "item"
            },
            {
                amount = 100,
                name = "water",
                type = "fluid"
            }
        },
        results = {
            {
                amount = 100,
                name = "sulfuric-acid",
                type = "fluid"
            }
        },
        show_amount_in_title = false,
        always_show_products = true,
    }
})

------------------------------------------------------------------------------------------------------------------------------------------------------
