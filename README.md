# Pokémon generation 3 GBA RNG calculator
This is a very simple application to use. I made it mainly because I couldn't find any application where I could specify the exact RNG frame of Pokemon Fire Red given the starting seed and get all of the results without going through a bunch of extra stuff.

Because all of the gen 3 GBA Pokemon games use the same formula to calculate RNG as well as having the same way to determine if an attack is a crit and determine damage roll multipliers, this RNG calculator should work with any of the GBA games, including Ruby, Sapphire, Fire Red, Leaf Green, and Emerald. All you need for it to work is the initial RNG seed. There is a very useful set of [lua scripts made by mkdasher](https://github.com/mkdasher/PokemonBizhawkLua) that can help you find the RNG seed on [BizHawk emulator](https://github.com/TASVideos/BizHawk), or you can manually find the RNG seed by locating it in memory (located at 0x02020000 in Fire Red and Leaf Green) on pretty much any emulator, including BizHawk. Mkdasher's lua scripts are also very useful for things other than just finding the RNG seed, so I'd definitely recommend checking it out if you're making a gen 3 TAS. This application can also generate the same RNG values given the seed as the 4th generation DS main-series games, including Diamond, Pearl, Platinum, Heart Gold, and Soul Silver, but I'm not sure if those games have the same way of determining whether an attack is a critical hit or determining the damage roll, so use caution if trying to use the calculator to find crit / roll frames in a gen 4 game. As far as compatibility with gens 1-2 go, they use completely different methods of calculating RNG values, and gens 5-8 use a 64-bit method of RNG calculation, which is completely incompatible with all of the kinds of integers used in this program, as they rely on being 32-bit integers in order for the calculations to work.

## Compatibility chart

|                | Fully compatible | Doesn't Work  | Semi-compatible*|Minor tweaks needed**|
| -------------  |:----------------:|:-------------:|:---------------:|:-------------------:|
| red            |                  | x             |                 |                     |
| blue           |                  | x             |                 |                     |
| yellow         |                  | x             |                 |                     |
| stadium        |                  | x             |                 |                     |
| stadium 2      |                  | x             |                 |                     |
| gold           |                  | x             |                 |                     |
| silver         |                  | x             |                 |                     |
| crystal        |                  | x             |                 |                     |
| ruby           | x                |               |                 |                     |
| sapphire       | x                |               |                 |                     |
| fire red       | x                |               |                 |                     |
| leaf green     | x                |               |                 |                     |
| emerald        | x                |               |                 |                     |
| colosseum      |                  |               |                 | x                   |
| xd             |                  | x             |                 |                     |
| diamond        |                  |               | x               |                     |
| pearl          |                  |               | x               |                     |
| platinum       |                  |               | x               |                     |
| heart gold     |                  |               | x               |                     |
| soul silver    |                  |               | x               |                     |
| battle rev     |                  | x             |                 |                     |
| black          |                  | x             |                 |                     |
| white          |                  | x             |                 |                     |
| black 2        |                  | x             |                 |                     |
| white 2        |                  | x             |                 |                     |
| x              |                  | x             |                 |                     |
| y              |                  | x             |                 |                     |
| omega ruby     |                  | x             |                 |                     |
| alpha sapphire |                  | x             |                 |                     |
| sun            |                  | x             |                 |                     |
| moon           |                  | x             |                 |                     |
| ultra sun      |                  | x             |                 |                     |
| ultra moon     |                  | x             |                 |                     |
| sword          |                  | x             |                 |                     |
| shield         |                  | x             |                 |                     |

*\*Semi-compatible means that the program can calculate the exact main RNG values used in these games, but I don't think it can correctly calculate which frames a crit happens on.*

*\**Minor tweaks needed means that the program could calculate the exact main RNG values used in the game, but would need the static RNG equation values changed, done by changing `RNGBaseNumOne` and `RNGBaseNumTwo` to the correct values in the source code, (in program.cs) and manually compiling it.*


## Why do I care so much about finding exact RNG values?

Put simply, I was tinkering around with TASing Fire Red when I realized that finding the exact frame needed to get a critical hit when it was necessary was a major pain, as just letting the game run so I could see what RNG values it generated was impractical due to the fact that the RNG frame advances twice per visual frame during a battle, but you need to be able to see the opposite polarity (if you are advancing on even numbers, you need to see the odd numbers and vice versa) of RNG frames to actually get a critical hit (because the RNG frame advances 3 times on the visual frame it's calculated).

## Why don't I just use RNGReporter instead of making a brand new application?

Well, you see, I actually do use [RNGReporter](https://github.com/Admiral-Fish/RNGReporter) quite a bit, and it's an amazing program. This application actually goes very nice when used in conjunction with RNGReporter, as you can find the frames you need to get Pokémon with great IVs and natures using that, while using this application to make finding the frame to attack and get critical hits and max damage rolls much easier, making the RNG manipulation aspect of creating a gen 3 Pokémon TAS a lot less trial-and-error and a lot more straightforward in general.

## How does one use this application?

Basically, just fill out all the boxes applicable to what you are searching for, then click enter to search. A new window will come up with all the results.

Some various in-application screenshots showing off the GUI-

![](https://raw.githubusercontent.com/jvhbv/fire-red-rng-calculator/tree/GUI/GUIProgram1.PNG)
![](https://raw.githubusercontent.com/jvhbv/fire-red-rng-calculator/tree/GUI/GUIProgram2.PNG)
![](https://raw.githubusercontent.com/jvhbv/fire-red-rng-calculator/tree/GUI/GUIProgram3.PNG)
![](https://raw.githubusercontent.com/jvhbv/fire-red-rng-calculator/tree/GUI/GUIProgram4.PNG)
![](https://raw.githubusercontent.com/jvhbv/fire-red-rng-calculator/tree/GUI/GUIProgram5.PNG)

## Great, now how do you go about installing it?

One of the useful things I find about the .NET framework console applications is that there is no installation needed for the actual application, although you must have [.NET 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472) installed in order to run the application. You can get direct downloads to the executable without having to manually compile it on the [releases](https://github.com/jvhbv/fire-red-rng-calculator/releases) page as more versions get released with more features and bug fixes.

## License
This software is protected under the [GNU General Public License v3.0](https://choosealicense.com/licenses/gpl-3.0/)
