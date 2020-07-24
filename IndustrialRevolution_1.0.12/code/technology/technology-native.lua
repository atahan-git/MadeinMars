------------------------------------------------------------------------------------------------------------------------------------------------------

-- DIR-specific non-backbone techs - costs added later
-- no need to specify numbered chains as prereqs, no need to specify prereqs that provide ingredients - conceptual prereqs only

add_new_minimal_tech("deadlock-advanced-batteries", "charged-advanced-battery", false, {"advanced-battery","charged-advanced-battery"}, {"battery"}) 
add_new_minimal_tech("deadlock-advanced-engine", "advanced-engine-unit", false, {"advanced-engine-unit"}, {"electric-engine"})
add_new_minimal_tech("deadlock-arc-turret", "arc-turret", false, {"arc-turret"}, {"military-3"}) 
add_new_minimal_tech("deadlock-autogun-turret", "autogun-turret", false, {"gun-turret"}, {"military"}) 
add_new_minimal_tech("deadlock-barrelling", "empty-barrel", false, {"empty-barrel"}, {"fluid-handling"}) 
add_new_minimal_tech("deadlock-bronze-construction", "steambot", false, {"burner-generator-equipment","bronze-roboport-equipment","steambot"}, {"heavy-armor"}, {{ type = "ghost-time-to-live", modifier = 36288000 }})
add_new_minimal_tech("deadlock-carbon-fibre", "carbon-plate", false, {"carbon-plate"}, {}) 
add_new_minimal_tech("deadlock-concrete-1", "concrete", true, {"concrete", "hazard-concrete"}, nil) 
add_new_minimal_tech("deadlock-concrete-2", "concrete", true, {"refined-concrete","refined-hazard-concrete"}, {}) 
add_new_minimal_tech("deadlock-electric-locomotives", "railway", true, {"electric-locomotive"}, {"battery","railway"}) 
add_new_minimal_tech("deadlock-electronics-1", "electronic-circuit", false, {"electronic-circuit","controller-mk1","computer-mk1","sensor","copper-foil"}, {"deadlock-steam-power"}) 
add_new_minimal_tech("deadlock-electronics-2", "advanced-circuit", false, {"advanced-circuit","controller-mk2","computer-mk2","gold-cable","gold-foil"}, {}) 
add_new_minimal_tech("deadlock-electronics-3", "processing-unit", false, {"processing-unit","controller-mk3","computer-mk3","glass-cable","glass-cable-heavy"}, {}) 
add_new_minimal_tech("deadlock-filtering-1", "simple-pollution-filter", false, {"simple-pollution-filter"}, {}) 
add_new_minimal_tech("deadlock-filtering-2", "standard-pollution-filter", false, {"standard-pollution-filter"}, {}) 
add_new_minimal_tech("deadlock-filtering-3", "advanced-pollution-filter", false, {"advanced-pollution-filter"}, {}) 
add_new_minimal_tech("deadlock-force-fields", "field-effector", false, {"field-effector"}, {}) 
add_new_minimal_tech("deadlock-forestry-1", "bronze-forestry", false, {"bronze-forestry","wood-sapling-growth","rubber-sapling-growth"}, {}) 
add_new_minimal_tech("deadlock-forestry-2", "iron-forestry", false, {"iron-forestry"}, {}) 
add_new_minimal_tech("deadlock-inserters-1", "inserters-1", false, {"inserter","long-handed-inserter","slow-filter-inserter"}, {"automation","logistics"}) 
add_new_minimal_tech("deadlock-inserters-2", "fast-inserter", true, {"fast-inserter","filter-inserter"}, {}) 
add_new_minimal_tech("deadlock-inserters-3", "inserters-3", false, {"stack-inserter","stack-filter-inserter"}, {}, {{type = "stack-inserter-capacity-bonus", modifier = 5}}) 
add_new_minimal_tech("deadlock-minigun", "minigun", false, {"minigun","titanium-belt"}, {"military-3","weapon-shooting-speed-4","physical-projectile-damage-4"},{{type = "ammo-damage", modifier = 0.8, ammo_category = "belt"}}) 
add_new_minimal_tech("deadlock-minigun-turret", "minigun-turret", false, {"minigun-turret"}, {"deadlock-minigun"},{{type = "turret-attack", modifier = 0.8, turret_id = "minigun-turret"}}) 
add_new_minimal_tech("deadlock-monowheel", "monowheel", false, {"monowheel"}, {"logistics"})
add_new_minimal_tech("deadlock-nuclear-fuel", "nuclear-fuel", false, {"nuclear-fuel"}, {"rocket-fuel","uranium-processing"}) 
add_new_minimal_tech("deadlock-photon-turret", "photon-turret", false, {"photon-turret"}, {"military-3"}) 
add_new_minimal_tech("deadlock-racing-monowheel", "racing-monowheel", false, {"racing-monowheel"}, {"battery"})
add_new_minimal_tech("deadlock-radar", "radar", false, {"radar","magnetron"}, {}) 
add_new_minimal_tech("deadlock-research-1", "improved-research", false, {"lab"}, {}) 
add_new_minimal_tech("deadlock-research-2", "quantum-lab", false, {"quantum-lab","quantum-ring"}, {}) 
add_new_minimal_tech("deadlock-robotower", "robotower", false, {"robotower","logistic-robot","logistic-chest-passive-provider","logistic-chest-storage"}, {"personal-roboport-equipment"}) 
add_new_minimal_tech("deadlock-scrapping-1", "copper-scrapper", false, {"copper-scrapper"}, {"automation"}) 
add_new_minimal_tech("deadlock-scrapping-2", "iron-scrapper", false, {"iron-scrapper"}, {"automation-2"}) 
add_new_minimal_tech("deadlock-solar-energy-1", "solar-panel", false, {"solar-panel"}, {"electric-energy-distribution-1"}) 
add_new_minimal_tech("deadlock-solar-energy-2", "solar-array", false, {"solar-array"}, {}) 
add_new_minimal_tech("deadlock-space-surveys-1", "ion-probe", false, {"ion-probe"}, {}) 
add_new_minimal_tech("deadlock-space-surveys-2", "impulse-probe", false, {"impulse-probe"}, {}) 
add_new_minimal_tech("deadlock-steel-wall", "steel-plate-wall", false, {"steel-plate-wall"}, {}) 
add_new_minimal_tech("deadlock-storage-1", "bronze-storage-depot", false, {"bronze-storage-depot"}, {}) 
add_new_minimal_tech("deadlock-storage-2", "steel-storage-depot", false, {"steel-storage-depot"}, {}) 

local suffixes = {"","-2","-3"}
local modules = {"effectivity","productivity","speed"}
for i=1,3 do
	local unlocks = {}
	for _,mod in pairs(modules) do
		table.insert(unlocks, "program-"..mod.."-module"..suffixes[i])
		table.insert(unlocks, "deprogram-"..mod.."-module"..suffixes[i])
	end
	add_new_minimal_tech("deadlock-modules-"..i, modules[i].."-module"..suffixes[i], false, unlocks, {"deadlock-electronics-"..i}) 
end

add_machine_mask_to_tech(add_new_minimal_tech("deadlock-storage-3", "logistics-depot", false, {"logistics-depot-passive", "logistics-depot-storage"}, {"deadlock-robotower"}), "logistics-depot-storage")
add_machine_mask_to_tech(add_new_minimal_tech("deadlock-storage-4", "logistics-depot", false, {"logistics-depot-active", "logistics-depot-requester", "logistics-depot-buffer"}, {"logistic-system"}), "logistics-depot-active")

local inserter_effects = {{type = "inserter-stack-size-bonus", modifier = 1}}
add_new_minimal_tech("deadlock-normal-inserter-capacity-bonus-1", "inserter-capacity", true, nil, {}, inserter_effects)
add_new_minimal_tech("deadlock-normal-inserter-capacity-bonus-2", "inserter-capacity", true, nil, {"deadlock-inserters-2"}, inserter_effects)

------------------------------------------------------------------------------------------------------------------------------------------------------
