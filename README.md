# RNG Pro Dice Roller

This project takes a random byte (0-255) stream as input from a TrueRNG Pro v2 (https://ubld.it/products/truerngpro) and converts it to a stream of dice rolls for different types of die. Dice implemented are d2 (coin flip), d4, d6, d8, d10, d12, d20 and d100.

Have tried to be as efficient as possible with converting bytes to dice rolls,e.g a single byte produces 8 coin flips (8 x 1 bit) or on average 2.8 d6 rolls (3 rolls if the byte value <=216 or 2 rolls for 217-252). 

Die rolls are put into a Concurrent Stack so they can be accessed in a threadsafe manner.

The main body of code runs through each die type and generates 10M rolls for each type and :
- shows a breakdown of rolls so you can check the die are balanced
- gives performance timings for each die type.

No driver is required for this - the RNG pro just provides a stream of Bytes to COM4 (you may need to confirm this in device manager and adjust if required), and should work with any other RNG which produces a stream of bytes.

Sample Fairness Check for D6<br/>
1: 1667752 rolls. Frequency 1:5.996095342712826   --  Variance -0.0039046572871743734<br/>
2: 1667631 rolls. Frequency 1:5.996530407506217   --  Variance -0.0034695924937828693<br/>
3: 1666816 rolls. Frequency 1:5.999462448164644   --  Variance -0.000537551835355643<br/>
4: 1665728 rolls. Frequency 1:6.003381104237906   --  Variance 0.003381104237906385<br/>
5: 1666112 rolls. Frequency 1:6.001997464756271   --  Variance 0.00199746475627105<br/>
6: 1665961 rolls. Frequency 1:6.002541476060964   --  Variance 0.0025414760609638876<br/>

Performance sample<br/>
die: 2    -   3,005,710 rolls per second<br/>
die: 4    -   1,262,466 rolls per second<br/>
die: 6    -   1,153,136 rolls per second<br/>
die: 8    -   835,003 rolls per second<br/>
die: 10    -  482,509 rolls per second<br/>
die: 12    -  481,834 rolls per second<br/>
die: 20    -  403,681 rolls per second<br/>
die: 100   -  215,415 rolls per second<br/>
