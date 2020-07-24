------------------------------------------------------------------------------------------------------------------------------------------------------

local fuel = table.deepcopy(data.raw.item["solid-fuel"])

fuel.name = "solid-fuel-yellow"
fuel.icon = get_icon_path("solid-fuel-yellow")
fuel.icon_size = DIR.icon_size
fuel.fuel_value = "15MJ"
fuel.fuel_emissions_multiplier = 1.25

data:extend({fuel})

fuel = table.deepcopy(data.raw.item["solid-fuel"])

fuel.name = "solid-fuel-red"
fuel.icon = get_icon_path("solid-fuel-red")
fuel.icon_size = DIR.icon_size
fuel.fuel_value = "12MJ"
fuel.fuel_emissions_multiplier = 1.5

data:extend({fuel})

------------------------------------------------------------------------------------------------------------------------------------------------------
