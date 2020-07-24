------------------------------------------------------------------------------------------------------------------------------------------------------

require("code.data.globals")
require("code.functions.functions-maths")
require("code.functions.functions-recipes")

------------------------------------------------------------------------------------------------------------------------------------------------------

-- Mark or remove any basic items that aren't used in recipes (must run before scrapping/incineration)
redundancy_scan()

-- More mod stuff (must run after redundancy)
require("code.mods.mods-datafinalfixes")

-- Generate scrapping and incineration/voiding recipes
require("code.items-recipes.recipes-scrapping")
require("code.items-recipes.recipes-incinerate")

-- Final, final pass on techs and recipes
require("code.technology.technology-prereq-convert")
final_recipes_pass()

-- Label ammo
ammo_pass()

------------------------------------------------------------------------------------------------------------------------------------------------------

