Solver for the Pushing Machine game for Android, solutions for Pushing Machine!
https://play.google.com/store/apps/details?id=com.reactor.pushingmachine


PushingMachineSolver: wrong number of parameters
PushingMachineSolver startingmoves filenamebase
Where startingnesting is the minimmun moves to search for and
datafilenamebase is the base name for 3 files:
'filenamebase'_data.txt is the data file
'filenamebase'_target.txt is the file with the targets
    (if there is no element over a target, it should be the same as the data)
'filenamebase'_output.txt is the file where the solution will be written
    the solution is both printed on the console and saved to this file

'filenamebase'_data.txt contains one line for each line in the puzzle.
and 'filenamebase'_target.txt contains the targets.
Use the following letters:
 - space for a blank spot
 - X for a wall
 - . for a target
 - o for a crate
 - 1 for a up pusher
 - 2 for a down pusher
 - 3 for a left pusher
 - 4 for a right pusher

You can find some sample files in the 'data' folder on the source

