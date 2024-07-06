using BirdsLibrary;

var BRANCH_COUNT = 14;
var BRANCH_SIZE = 5;
var BIRD_TYPE_COUNT = 10;
var BIRD_COUNT = 50;

var birdGame = new BirdsGame(BRANCH_COUNT, BRANCH_SIZE, BIRD_TYPE_COUNT, BIRD_COUNT);

while (true)
{
    PrintBoard(birdGame);

    if (birdGame.GameWon())
    {
        break;
    }

    while (true)
    {
        Console.WriteLine("Enter [fromBranch] [toBranch]");

        var input = Console.ReadLine() ?? "";
        var splitted = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);

        if (splitted.Length != 2)
        {
            continue;
        }

        if (!int.TryParse(splitted[0], out var fromBranch) || !int.TryParse(splitted[1], out var toBranch))
        {
            continue;
        }

        if (birdGame.MoveBranches(fromBranch, toBranch))
        {
            break;
        }
    }
}


string BranchToString<T>(IList<T> branches, int branchIndex) where T : IList<int?>
{
    var birdChars = "0123456789abcdefghijklmnopqrstuvwxyz";
    var branch = branches[branchIndex];
    var birdsString = string.Join("", branch.Where(b => b is not null).Select(b => birdChars[b.Value]));

    return $"[{branchIndex}]:[{birdsString}]";
}

void PrintBoard(BirdsGame birdGame)
{
    Console.Clear();

    var _branches = birdGame.Branches;

    for (int i = 0; i < _branches.Count; i += 2)
    {
        Console.Write(BranchToString(_branches, i));

        if (i + 1 < _branches.Count)
        {
            Console.Write($" {BranchToString(_branches, i + 1)}");
        }

        Console.WriteLine();
    }

    Console.WriteLine();
}