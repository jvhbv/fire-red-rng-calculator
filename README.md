# Pokémon generation 3 GBA RNG calculator
This is a very simple application to use. I made it mainly because I couldn't find any application where I could specify the exact RNG frame of Pokemon Fire Red given the starting seed and get all of the results without going through a bunch of extra stuff.

Because all of the gen 3 GBA Pokemon games use the same formula to calculate RNG as well as having the same way to determine if an attack is a crit and determine damage roll multipliers, this RNG calculator should work with any of the GBA games, including Ruby, Sapphire, Fire Red, Leaf Green, and Emerald. All you need for it to work is the initial RNG seed. There is a very useful set of [lua scripts made by mkdasher](https://github.com/mkdasher/PokemonBizhawkLua) that can help you find the RNG seed on [BizHawk emulator](https://github.com/TASVideos/BizHawk), or you can manually find the RNG seed by locating it in memory (located at 0x02020000 in Fire Red and Leaf Green) on pretty much any emulator, including BizHawk. Mkdasher's lua scripts are also very useful for things other than just finding the RNG seed, so I'd definitely recommend checking it out if you're making a gen 3 TAS.

## Why do I care so much about finding exact RNG values?

Put simply, I was tinkering around with TASing Fire Red when I realized that finding the exact frame needed to get a critical hit when it was necessary was a major pain, as just letting the game run so I could see what RNG values it generated was impractical due to the fact that the RNG frame advances twice per visual frame during a battle, but you need to be able to see the opposite polarity (if you are advancing on even numbers, you need to see the odd numbers and vice versa) of RNG frames to actually get a critical hit (because the RNG frame advances 3 times on the visual frame it's calculated).

## Why don't I just use RNGReporter instead of making a brand new application?

Well, you see, I actually do use [RNGReporter](https://github.com/Admiral-Fish/RNGReporter) quite a bit, and it's an amazing program. This application actually goes very nice when used in conjunction with RNGReporter, as you can find the frames you need to get Pokémon with great IVs and natures using that, while using this application to make finding the frame to attack and get critical hits and max damage rolls much easier, making the RNG manipulation aspect of creating a gen 3 Pokémon TAS a lot less trial-and-error and a lot more straightforward in general.

## How does one use this application?

Basically, just follow all the prompts that the terminal application gives, filling in the exact hex value of the starting seed, the number of frames you want to calculate, and typing yes or anything else into the prompt to calculate crits only and then doing the same thing for whether you only want to display crit max damage pairs if you answered yes to calculating crits only.

The program just checks if the first letter in your response to the prompts to calculate crits only and calculate crit max damage pairs is either "y" or "Y", specifically at `if (critAsk.IndexOf("y", 0, 1) == 0 || critAsk.IndexOf("Y", 0, 1) == 0)`, having the first letter be anything else will just assume that you do not want to calculate crits only, and will display all results. A quick thing to note is that this program is still very much in alpha stages, so if you don't type anything on the question to calculate crits or the question to display crit max damage pairs, the application crashes, and if you type things in that the application doesn't like on the other prompts, it will also crash.

Here's an example of how to use the application:

```
Enter the initial seed hex value:
23AC
Enter the number of times to repeat:
1000
Would you like to search for only crit frames?
y
Would you like to search for only max roll crit frame pairs?
y
```

Outputs:
```
172: 0x452042F8
177: 0xDFC0079F

333: 0xD8B0622B
338: 0x01F0CEC6

590: 0xB720BFC2
595: 0x160093C1

791: 0xBCE0A875
796: 0x17C05E68

857: 0x9F3020A7
862: 0xDBF01252

868: 0xBA0055B0
873: 0x6C203D77

Press enter to continue.
```

Another example for good measure:
```
Enter the initial seed hex value:
23AC
Enter number of times to repeat:
10
Would you like to search for only crit frames?
n
```

Outputs:
```
1: 0x4DF5F8AF
2: 0xE9DA94F6
3: 0xD7B8C131
4: 0x016D9050
5: 0xEB7F3283
6: 0x5ADECC3A
7: 0x41F30125
8: 0x666F2334
9: 0xE8443597
10: 0x772933BE
Press enter to continue.
```

## Great, now how do you go about installing it?

One of the useful things I find about the .NET framework console applications is that there is no installation needed for the actual application, although you must have [.NET 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472) installed in order to run the application. You can get direct downloads to the executable without having to manually compile it on the [releases](https://github.com/jvhbv/fire-red-rng-calculator/releases) page as more versions get released with more features and bug fixes.

## Contribution

As-is, this software is created solely by me. If you would like to contribute by adding ideas, knowledge, bug fixes, or want to modify the source code for your own use, you are welcome to. Pull requests are welcome, and if you find bugs that have not already been reported, you can open a [new issue](https://github.com/jvhbv/fire-red-rng-calculator/issues) to let me know about it. If you modify and publish the source code, make sure to follow the guidelines of the [GNU GPL v3.0](https://choosealicense.com/licenses/gpl-3.0/) license, and aside from that, all ideas are welcome so I can make this program be as great as it can be.

## License
This software is protected under the [GNU General Public License v3.0](https://choosealicense.com/licenses/gpl-3.0/)
