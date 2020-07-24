------------------------------------------------------------------------------------------------------------------------------------------------------

-- chargeable batteries

replace_recipe_ingredients("battery", {
	{type = "item", name = "iron-tube", amount = 1},
	{type = "item", name = "tin-plate", amount = 1},
	{type = "item", name = "copper-plate", amount = 1},
	{type = "fluid", name = "sulfuric-acid", amount = 25}
}, 1, "crafting-with-fluid", 2)

data:extend({
    {
        type = "fuel-category",
        name = "battery",
    },
    {
        type = "recipe",
        name = "charged-battery",
        results = {
            { type = "item", name = "charged-battery", amount = 1, probability = 0.98 }
        },
        order = "h[c]",
        category = "charging",
        enabled = false,
        always_show_made_in = true,
        show_amount_in_title = false,
        always_show_products = true,
        ingredients = {
            {"battery",1},
        },
        energy_required = 10,
        overload_multiplier = 200,
        hide_from_stats = true,
        allow_decomposition = false,
    },
    {
        type = "recipe",
        name = "advanced-battery",
        results = {
            { type = "item", name = "advanced-battery", amount = 1 }
        },
        order = "h[d]",
        category = "crafting-with-fluid",
        enabled = false,
        show_amount_in_title = false,
        ingredients = {
            {type = "item", name = "steel-tube", amount = 1},
            {type = "item", name = "lead-plate", amount = 4},
            {type = "item", name = "copper-plate", amount = 4},
            {type = "fluid", name = "sulfuric-acid", amount = 100},
        },
        energy_required = 4,
    },
    {
        type = "recipe",
        name = "charged-advanced-battery",
        results = {
            { type = "item", name = "charged-advanced-battery", amount = 1, probability = 0.99 }
        },
        order = "h[e]",
        category = "charging",
        enabled = false,
        always_show_made_in = true,
        show_amount_in_title = false,
        always_show_products = true,
        ingredients = {
            {"advanced-battery",1},
        },
        energy_required = 40,
        overload_multiplier = 50,
        hide_from_stats = true,
        allow_decomposition = false,
    },
	{
		type = "item",
		name = "charged-battery",
		icon = get_icon_path("charged-battery", DIR.icon_size),
		icon_size = DIR.icon_size,
		stack_size = 100,
		order = "h[c]",
		subgroup = "intermediate-product",
		burnt_result = "battery",
		fuel_value = "5MJ",
		fuel_category = "battery",
		fuel_acceleration_multiplier = 1.1,
		fuel_emissions_multiplier = 0.05,
		fuel_glow_color = {0.75,0.75,1.0,1.0},
	},
	{
		type = "item",
		name = "advanced-battery",
		icon = get_icon_path("advanced-battery", DIR.icon_size),
		icon_size = DIR.icon_size,
		stack_size = 50,
		order = "h[d]",
		subgroup = "intermediate-product",
	},
	{
		type = "item",
		name = "charged-advanced-battery",
		icon = get_icon_path("charged-advanced-battery", DIR.icon_size),
		icon_size = DIR.icon_size,
		stack_size = 50,
		order = "h[e]",
		subgroup = "intermediate-product",
		burnt_result = "advanced-battery",
		fuel_value = "20MJ",
		fuel_category = "battery",
		fuel_acceleration_multiplier = 1.2,
		fuel_emissions_multiplier = 0.05,
		fuel_glow_color = {0.75,0.75,1.0,1.0},
	},
})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- electric loco

data:extend({
	{
		type = "item-with-entity-data",
		name = "electric-locomotive",
		icon = get_icon_path("electric-locomotive", DIR.icon_size),
		icon_size = DIR.icon_size,
		order = "a[train-system]-f[electric-locomotive]",
		place_result = "electric-locomotive",
		stack_size = 5,
		subgroup = "transport",
	},
    {
        type = "recipe",
        name = "electric-locomotive",
        result = "electric-locomotive",
        result_count = 1,
        category = "crafting",
        enabled = false,
        always_show_made_in = true,
        ingredients = {
            {"electric-engine-unit",3},
            {"steel-plate",10},
            {"steel-chassis-small",2},
            {"controller-mk2",1},
        },
        energy_required = 4,
    },
})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- battery discharge equipment

data:extend({
    {
        type = "item",
        name = "battery-discharge-equipment",
        placed_as_equipment_result = "battery-discharge-equipment",
        order = "a[energy-source]-b[battery]",
        stack_size = 10,
        icon = get_icon_path("battery-discharge-equipment", DIR.icon_size),
        icon_size = DIR.icon_size,
        subgroup = "equipment",
    },
    {
        type = "recipe",
        name = "battery-discharge-equipment",
        result = "battery-discharge-equipment",
        result_count = 1,
        category = "crafting",
        enabled = false,
        ingredients = {
            {"iron-chassis-small",1},
            {"iron-tube",5},
            {"copper-cable",10},
            {"controller-mk1",1},
        },
        energy_required = 4,
    },
    {
        type = "generator-equipment",
        name = "battery-discharge-equipment",
        categories = {
            "armor"
        },
        energy_source = {
            type = "electric",
            usage_priority = "secondary-output"
        },
        power = "200kW",
        shape = {
            height = 2,
            type = "full",
            width = 2
        },
        sprite = {
            filename = get_icon_path("battery-discharge-equipment", 128),
            height = 128,
            priority = "medium",
            width = 128
        },
        burner = {
            emissions_per_minute = 0,
            fuel_category = "battery",
            fuel_inventory_size = 1,
            burnt_inventory_size = 1,
            type = "burner"
        },
    }
})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- change the fusion reactor into a fission reactor

local reactor = data.raw["generator-equipment"]["fusion-reactor-equipment"]
reactor.power = "1.6MW"
reactor.burner = {
    emissions_per_minute = 0,
    fuel_category = "nuclear",
    fuel_inventory_size = 1,
    burnt_inventory_size = 1,
    type = "burner"
}
data.raw.item["fusion-reactor-equipment"].order = "a[energy-source]-z[nuclear]"

------------------------------------------------------------------------------------------------------------------------------------------------------

-- solar panel

local panel = data.raw["solar-panel-equipment"]["solar-panel-equipment"]
panel.power = "40kW"

------------------------------------------------------------------------------------------------------------------------------------------------------

-- bronze burner generator equipment

data:extend({
    {
        type = "item",
        name = "burner-generator-equipment",
        placed_as_equipment_result = "burner-generator-equipment",
        order = "a[energy-source]-a[aardvark-generator]",
        stack_size = 10,
        icon = get_icon_path("burner-generator", DIR.icon_size),
        icon_size = DIR.icon_size,
        subgroup = "equipment",
    },
    {
        type = "recipe",
        name = "burner-generator-equipment",
        result = "burner-generator-equipment",
        result_count = 1,
        category = "crafting",
        enabled = false,
        ingredients = {
            {"bronze-chassis-small",1},
            {"copper-motor",1},
            {"bronze-tube",3},
        },
        energy_required = 3,
    },
    {
        type = "generator-equipment",
        name = "burner-generator-equipment",
        categories = {"armor"},
        energy_source = {
            type = "electric",
            usage_priority = "secondary-output"
        },
        power = "100kW",
        shape = {
            height = 2,
            type = "full",
            width = 2
        },
        sprite = {
            filename = get_icon_path("burner-generator", 128),
            height = 128,
            priority = "medium",
            width = 128
        },
        burner = {
            emissions_per_minute = 2,
            fuel_category = "chemical",
            fuel_inventory_size = 1,
            burnt_inventory_size = 1,
            type = "burner"
        },
    }
})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- iron burner generator equipment

local item = table.deepcopy(data.raw.item["burner-generator-equipment"])
item.name = "iron-burner-generator-equipment"
item.placed_as_equipment_result = "iron-burner-generator-equipment"
item.order = "a[energy-source]-a[b]"
item.icon = get_icon_path("iron-burner-generator")
data:extend({item})

local recipe = table.deepcopy(data.raw.recipe["burner-generator-equipment"])
recipe.name = "iron-burner-generator-equipment"
recipe.result = "iron-burner-generator-equipment"
recipe.ingredients = {
    {"iron-chassis-small",1},
    {"iron-motor",1},
    {"iron-piston",4},
}
data:extend({recipe})

local equipment = table.deepcopy(data.raw["generator-equipment"]["burner-generator-equipment"])
equipment.name = "iron-burner-generator-equipment"
equipment.power = "125kW"
equipment.sprite.filename = get_icon_path("iron-burner-generator",128)
data:extend({equipment})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- steambots

data:extend({
    {
        type = "item",
        name = "steambot",
        localised_name = {"item-name.steambot"},
        icon = get_icon_path("steambot",64),
        icon_size = DIR.icon_size,
        stack_size = 50,
        subgroup = "deadlock-robots",
        order = "a",
        place_result = "steambot",
    },
    {
        type = "recipe",
        name = "steambot",
        localised_name = {"item-name.steambot"},
        category = "crafting",
        enabled = false,
        ingredients = {
            {"copper-motor",1},
            {"bronze-chassis-small",1},
            {"bronze-rod",4},
        },
        result = "steambot",
        result_count = 1,
        energy_required = 3.0,
    }
})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- bronze personal roboport

data:extend({
    {
        type = "item",
        name = "bronze-roboport-equipment",
        placed_as_equipment_result = "bronze-roboport-equipment",
        order = "e[robotics]-a[aardvark-personal-roboport-equipment]",
        stack_size = 10,
        icon = get_icon_path("bronze-personal-roboport", DIR.icon_size),
        icon_size = DIR.icon_size,
        subgroup = "equipment",
    },
    {
        type = "recipe",
        name = "bronze-roboport-equipment",
        result = "bronze-roboport-equipment",
        result_count = 1,
        category = "crafting",
        enabled = false,
        ingredients = {
            {"bronze-chassis-small",1},
            {"copper-gear-wheel",4},
            {"copper-tube",4},
            {"bronze-chest",1},
        },
        energy_required = 3,
    },
    {
        type = "roboport-equipment",
        name = "bronze-roboport-equipment",
        take_result = "bronze-roboport-equipment",
        sprite =
        {
            filename = get_icon_path("bronze-personal-roboport",128),
            width = 128,
            height = 128,
            priority = "medium"
        },
        shape =
        {
            width = 2,
            height = 2,
            type = "full"
        },
        energy_source =
        {
            type = "electric",
            buffer_capacity = "5MJ",
            input_flow_limit = "100KW",
            usage_priority = "secondary-input"
        },
        charging_energy = "200kW",
        robot_limit = 10,
        construction_radius = 15,
        spawn_and_station_height = 0.4,
        charge_approach_distance = 2.6,
        recharging_animation =
        {
            filename = "__base__/graphics/entity/smoke-fast/smoke-fast.png",
            priority = "high",
            width = 50,
            height = 50,
            frame_count = 16,
            scale = 1,
            animation_speed = 0.5
        },
        recharging_light = {intensity = 0.4, size = 5},
        stationing_offset = {0, -0.6},
        charging_station_shift = {0, 0.5},
        charging_station_count = 2,
        charging_distance = 1.6,
        charging_threshold_distance = 5,
        categories = {"armor"},
        robots_shrink_when_entering_and_exiting = true,
    }
})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- spell it out

function add_generator_manager_tip_to_item(item)
	if data.raw.item[item] then
		data.raw.item[item].localised_description = { "", {"item-description."..item}, " ", {"item-description.generator-manager-tip"} }
	end
end

add_generator_manager_tip_to_item("burner-generator-equipment")
add_generator_manager_tip_to_item("iron-burner-generator-equipment")
add_generator_manager_tip_to_item("battery-discharge-equipment")
add_generator_manager_tip_to_item("fusion-reactor-equipment")

------------------------------------------------------------------------------------------------------------------------------------------------------

-- should be a quickbar toggle, innit

data.raw["belt-immunity-equipment"]["belt-immunity-equipment"].energy_consumption = "10kW"

------------------------------------------------------------------------------------------------------------------------------------------------------