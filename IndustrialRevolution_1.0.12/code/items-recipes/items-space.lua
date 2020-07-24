------------------------------------------------------------------------------------------------------------------------------------------------------

-- SPACE and shit

------------------------------------------------------------------------------------------------------------------------------------------------------

-- rocket launches

replace_recipe_ingredients("rocket-part", {{"steel-bulkhead",10},{"rocket-control-unit",10},{"rocket-fuel",10},{"low-density-structure",10}})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- vanilla satellite

replace_recipe_ingredients("satellite", {{"steel-bulkhead",40}, {"low-density-structure",120},{"computer-mk3",4},{"solar-array",32},{"accumulator",8},{"sensor",16}}, 1, "advanced-crafting", 60)
data.raw.item["satellite"].stack_size = 1
data.raw.item["satellite"].rocket_launch_product = {"space-science-pack",1000}
replace_item_icon("satellite")
change_item_subgroup("satellite", "space-products")
change_item_order("satellite", "z-satellite-a")

------------------------------------------------------------------------------------------------------------------------------------------------------

-- ion probe

data:extend({
	{
		type = "item",
		name = "ion-probe",
		localised_description = {"item-description.satellite"},
		order = "z-satellite-b",
		stack_size = 1,
		icon = get_icon_path("ion-probe", DIR.icon_size),
		icon_size = DIR.icon_size,
		subgroup = "space-products",
		rocket_launch_product = {"space-science-pack",2000},	
	},
	{
		type = "recipe",
		name = "ion-probe",
		results = {
			{type = "item", name = "ion-probe", amount = 1},
		},
		category = "endgame-crafting",
		enabled = false,
		ingredients = {
			{type = "item", name = "titanium-bulkhead", amount = 160},
			{type = "item", name = "low-density-structure", amount = 160},
			{type = "item", name = "computer-mk3", amount = 8},
			{type = "item", name = "accumulator", amount = 16},
			{type = "item", name = "sensor", amount = 48},
			{type = "item", name = "magnetron", amount = 8},
		},
		energy_required = 120,
	},
})

------------------------------------------------------------------------------------------------------------------------------------------------------

-- impulse probe

data:extend({
	{
		type = "item",
		name = "impulse-probe",
		localised_description = {"item-description.satellite"},
		order = "z-satellite-c",
		stack_size = 1,
		icon = get_icon_path("impulse-probe", DIR.icon_size),
		icon_size = DIR.icon_size,
		subgroup = "space-products",
		rocket_launch_product = {"space-science-pack",4000},	
	},
	{
		type = "recipe",
		name = "impulse-probe",
		results = {
			{type = "item", name = "impulse-probe", amount = 1},
		},
		category = "endgame-crafting",
		enabled = false,
		ingredients = {
			{type = "item", name = "duranium-bulkhead", amount = 240},
			{type = "item", name = "computer-mk3", amount = 12},
			{type = "item", name = "fusion-reactor-equipment", amount = 3},
			{type = "item", name = "uranium-fuel-cell", amount = 24},
			{type = "item", name = "accumulator", amount = 32},
			{type = "item", name = "quantum-ring", amount = 1},
		},
		energy_required = 180,
	},
})

------------------------------------------------------------------------------------------------------------------------------------------------------
