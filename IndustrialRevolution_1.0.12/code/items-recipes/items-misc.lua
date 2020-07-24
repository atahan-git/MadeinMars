------------------------------------------------------------------------------------------------------------------------------------------------------

-- rubber & saplings

data:extend({
    {
        type = "item",
        name = "wood-sapling",
        order = "c",
        stack_size = 100,
        icon = get_icon_path("wood-sapling", DIR.icon_size),
        icon_size = DIR.icon_size,
        subgroup = "wood",
    },
    {
        type = "item",
        name = "rubber-sapling",
        order = "d",
        stack_size = 100,
        icon = get_icon_path("rubber-sapling", DIR.icon_size),
        icon_size = DIR.icon_size,
        subgroup = "wood",
    },
    {
        type = "recipe",
        name = "wood-sapling-growth",
        results = {
            {type = "item", name = "wood", amount = 40},
            {type = "item", name = "wood-sapling", amount_min = 10, amount_max = 11},
        },
        category = "forestry",
        subgroup = "fluid-recipes-2",
        order = "za",
        always_show_made_in = true,
        enabled = false,
        ingredients = {
            {type = "item", name = "wood-sapling", amount = 10},
        },
        allow_decomposition = false,
        energy_required = 600,
        icons = {
            { icon = get_icon_path("wood", DIR.icon_size), icon_size = DIR.icon_size },
        },
    },
    {
        type = "recipe",
        name = "rubber-sapling-growth",
        results = {
            {type = "item", name = "rubber-wood", amount = 40},
            {type = "item", name = "rubber-sapling", amount_min = 10, amount_max = 11},
        },
        category = "forestry",
        subgroup = "fluid-recipes-2",
        order = "zb",
        always_show_made_in = true,
        enabled = false,
        ingredients = {
            {type = "item", name = "rubber-sapling", amount = 10},
        },
        allow_decomposition = false,
        energy_required = 600,
        icons = {
            { icon = get_icon_path("rubber-wood", DIR.icon_size), icon_size = DIR.icon_size },
        },
    }
})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- controllers & computers 1-3

replace_recipe_ingredients("electronic-circuit", {{"glass-plate", 1}, {"copper-foil", 3}}, 1)
replace_recipe_ingredients("advanced-circuit", {{"plastic-bar", 1}, {"gold-foil", 3}, {"electronic-circuit",2}}, 1)
replace_recipe_ingredients("processing-unit", {{"plastic-bar", 1}, {"solder", 1}, {"glass-cable",1}, {"advanced-circuit",2}}, 1, "advanced-crafting", 1)

local vanilla_circuits = {"electronic-circuit", "advanced-circuit", "processing-unit"}
local controller_wire = {"copper-cable", "gold-cable", "glass-cable"}
local computer_wire = {"copper-cable", "gold-cable", "glass-cable-heavy"}
local chassis = {"iron", "steel", "steel"}
local factor = 8
local hues = {0.333,1,0.667}

for i = 1,3 do

    local speed = 2^(i-1)

    -- circuit
    replace_item_icon(vanilla_circuits[i])
    change_item_subgroup(vanilla_circuits[i], "deadlock-electronics")
    data.raw.recipe[vanilla_circuits[i]].crafting_machine_tint = get_crafting_tints_from_hue(hues[i],1)
    data.raw.recipe[vanilla_circuits[i]].energy_required = speed
    if data.raw.recipe[vanilla_circuits[i]].normal then data.raw.recipe[vanilla_circuits[i]].normal.energy_required = speed end
    if data.raw.recipe[vanilla_circuits[i]].expensive then data.raw.recipe[vanilla_circuits[i]].expensive.energy_required = speed end

    -- controller
    local name = "controller-mk"..i
    local speed = 2^(i+1)
    local ingredients = {
        {vanilla_circuits[i], factor},
        {controller_wire[i], factor},
    }
    data:extend({
        {
            type = "item",
            name = name,
            order = "x"..i,
            stack_size = 100,
            icon = get_icon_path(name, DIR.icon_size),
            icon_size = DIR.icon_size,
            subgroup = "deadlock-electronics",
        },
        {
            type = "recipe",
            name = name,
            order = "x"..i,
            result = name,
            result_count = 1,
            category = (i<3) and "crafting" or "advanced-crafting",
            enabled = false,
            ingredients = ingredients,
            energy_required = speed,
            crafting_machine_tint = get_crafting_tints_from_hue(hues[i],1),
        }
    })

    -- computer
    name = "computer-mk"..i
    speed = 2^(i+3)
    local ingredients = {
        {"controller-mk"..i, factor},
        {computer_wire[i], (i<3) and factor*(2^(i-1)) or 8},
        {chassis[i].."-chassis-small",1},
    }
    data:extend({
        {
            type = "item",
            name = name,
            order = "z"..i,
            stack_size = 100,
            icon = get_icon_path(name, DIR.icon_size),
            icon_size = DIR.icon_size,
            subgroup = "deadlock-electronics",
        },
        {
            type = "recipe",
            name = name,
            order = "z"..i,
            result = name,
            result_count = 1,
            category = (i<3) and "crafting" or "advanced-crafting",
            enabled = false,
            ingredients = ingredients,
            energy_required = speed,
            crafting_machine_tint = get_crafting_tints_from_hue(hues[i],1),
        }
    })
end

------------------------------------------------------------------------------------------------------------------------------------------------------

-- sensor

data:extend({
    {
        type = "item",
        name = "sensor",
        order = "l[aardvark]",
        stack_size = 100,
        icon = get_icon_path("sensor", DIR.icon_size),
        icon_size = DIR.icon_size,
        subgroup = "intermediate-product",
    },
    {
        type = "recipe",
        name = "sensor",
        result = "sensor",
        result_count = 1,
        category = "crafting",
        enabled = false,
        ingredients = {
            {"electronic-circuit",1},
            {"glass-ingot",1},
            {"copper-plate",1},
        },
        energy_required = 2,
    }
})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- magnetron

data:extend({
    {
        type = "item",
        name = "magnetron",
        order = "l[byzantine]",
        stack_size = 50,
        icon = get_icon_path("magnetron", DIR.icon_size),
        icon_size = DIR.icon_size,
        subgroup = "intermediate-product",
    },
    {
        type = "recipe",
        name = "magnetron",
        result = "magnetron",
        result_count = 1,
        category = "crafting",
        enabled = false,
        ingredients = {
            {"iron-ingot",8},
            {"iron-plate",4},
            {"copper-cable",16},
        },
        energy_required = 4,
    }
})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- gyroscope

data:extend({
    {
        type = "item",
        name = "gyroscope",
        order = "l[bzzzzz]",
        stack_size = 100,
        icon = get_icon_path("gyroscope", DIR.icon_size),
        icon_size = DIR.icon_size,
        subgroup = "intermediate-product",
    },
    {
        type = "recipe",
        name = "gyroscope",
        result = "gyroscope",
        result_count = 1,
        category = "crafting",
        enabled = false,
        ingredients = {
            {"steel-motor",1},
            {"steel-rod",4},
            {"steel-gear-wheel",2},
            {"electronic-circuit",1},
        },
        energy_required = 2,
    }
})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- laser

data:extend({
    {
        type = "item",
        name = "laser",
        order = "l[cactus]",
        stack_size = 50,
        icon = get_icon_path("laser", DIR.icon_size),
        icon_size = DIR.icon_size,
        subgroup = "intermediate-product",
    },
    {
        type = "recipe",
        name = "laser",
        result = "laser",
        result_count = 1,
        category = "crafting",
        enabled = false,
        ingredients = {
            {"ruby-gem",4},
            {"steel-ring",1},
            {"steel-tube",1},
            {"advanced-circuit",1},
        },
        energy_required = 4,
    }
})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- effector

data:extend({
    {
        type = "item",
        name = "field-effector",
        order = "l[dandelion]",
        stack_size = 50,
        icon = get_icon_path("field-effector", DIR.icon_size),
        icon_size = DIR.icon_size,
        subgroup = "intermediate-product",
    },
    {
        type = "recipe",
        name = "field-effector",
        result = "field-effector",
        result_count = 1,
        category = "crafting",
        enabled = false,
        ingredients = {
            {"sapphire-gem",20},
            {"steel-tube",2},
            {"gold-foil",4},
            {"processing-unit",1},
        },
        energy_required = 5,
    }
})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- graphite electrode

data:extend({
    {
        type = "item",
        name = "graphite-electrode",
        order = "x",
        stack_size = 50,
        icon = get_icon_path("graphite-electrode", DIR.icon_size),
        icon_size = DIR.icon_size,
        subgroup = "fluid-recipes",
    },
    {
        type = "recipe",
        name = "graphite-electrode",
        result = "graphite-electrode",
        result_count = 1,
        category = "smelting-2",
        enabled = false,
        ingredients = {
            {"solid-fuel",48},
            {"copper-rod",16},
        },
        energy_required = 32,
        subgroup = "fluid-recipes-2",
    }
})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- dummy ammo item for signs

data:extend({
    {
        type = "item",
        name = "physical-projectiles",
        order = "z[zzzzz]",
        stack_size = 1,
        icon = get_icon_path("physical-projectiles", DIR.icon_size),
        icon_size = DIR.icon_size,
        subgroup = "ammo",
        flags = {"hidden"},
    },
})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- quantum ring

data:extend({
    {
        type = "item",
        name = "quantum-ring",
        order = "aardvark-kb-zzz",
        stack_size = 50,
        icon = get_icon_path("quantum-ring", DIR.icon_size),
        icon_size = DIR.icon_size,
        subgroup = "space-products",
    },
    {
        type = "recipe",
        name = "quantum-ring",
        result = "quantum-ring",
        result_count = 1,
        category = "endgame-crafting",
        enabled = false,
        ingredients = {
            {"duranium-ring",16},
            {"field-effector",16},
            {"duranium-rod",32},
            {"chromium-plate-heavy",32},
        },
        energy_required = 16,
    }
})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- junction box

data:extend({
    {
        type = "item",
        name = "junction-box",
        order = "aardvark-d-zzz",
        stack_size = 50,
        icon = get_icon_path("junction-box", DIR.icon_size),
        icon_size = DIR.icon_size,
        subgroup = "intermediate-product",
    },
    {
        type = "recipe",
        name = "junction-box",
        result = "junction-box",
        result_count = 1,
        category = "crafting",
        enabled = false,
        ingredients = {
            {"steel-chassis-small",1},
            {"copper-cable-heavy",4},
            {"solder",4},
        },
        energy_required = 2,
    }
})

------------------------------------------------------------------------------------------------------------------------------------------------------
