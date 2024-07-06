using System.Collections.ObjectModel;

namespace BirdsLibrary;

public class BirdsGame
{
    public readonly Random _random = new();

    public int BranchCount { get; }
    public int BranchSize { get; }

    private int?[][] _branches;
    public ReadOnlyCollection<ReadOnlyCollection<int?>> Branches =>
        _branches.Select(b => b.AsReadOnly()).ToList().AsReadOnly();

    public BirdsGame(int branchCount, int branchSize, int birdTypeCount, int birdCount)
    {
        BranchCount = branchCount;
        BranchSize = branchSize;

        _branches = new int?[branchCount][];

        for (int branchIndex = 0; branchIndex < branchCount; branchIndex++)
        {
            _branches[branchIndex] = new int?[branchSize];
        }

        var birds = Enumerable.Range(0, birdTypeCount)
                              .Select(t => Enumerable.Repeat(t, birdCount / birdTypeCount))
                              .SelectMany(ba => ba)
                              .OrderBy(b => _random.Next())
                              .ToList();

        for (int birdIndex = 0; birdIndex < birds.Count; birdIndex++)
        {
            var randomBranch = _branches[_random.Next(BranchCount)];
            var firstFreeSpace = Array.FindIndex(randomBranch, b => b is null);

            if (firstFreeSpace == -1)
            {
                birdIndex--;
            }
            else
            {
                randomBranch[firstFreeSpace] = birds[birdIndex];
            }
        }
    }

    public bool BranchOutOfBounds(int branchIndex)
    {
        return branchIndex < 0 || branchIndex >= _branches.Length;
    }

    public bool BranchIsEmpty(int branchIndex)
    {
        return _branches[branchIndex].All(b => b is null);
    }

    public bool MoveBranches(int fromBranch, int toBranch)
    {
        if (BranchOutOfBounds(fromBranch) || BranchOutOfBounds(toBranch))
        {
            return false;
        }

        if (BranchIsEmpty(fromBranch))
        {
            return false;
        }

        var lastBirdFrom = GetLastBird(fromBranch);
        var lastBirdTo = GetLastBird(toBranch);

        if (lastBirdFrom != lastBirdTo && lastBirdTo is not null)
        {
            return false;
        }

        var takeFromCount = GetSameOnBranchFromEnd(fromBranch, lastBirdFrom);
        var emptyToCount = GetEmptyCountOnBranch(toBranch);

        if (emptyToCount == 0)
        {
            return false;
        }

        var actualTakeCount = Math.Min(takeFromCount, emptyToCount);
        var fromBirdCount = Array.FindLastIndex(_branches[fromBranch], b => b is not null);
        var toFirstFree = Array.FindIndex(_branches[toBranch], b => b is null);

        for (int takeBird = 0; takeBird < actualTakeCount; takeBird++)
        {
            _branches[fromBranch][fromBirdCount - takeBird] = null;
            _branches[toBranch][toFirstFree + takeBird] = lastBirdFrom;
        }

        if (_branches[toBranch].All(b => b == lastBirdFrom))
        {
            _branches[toBranch] = new int?[BranchSize];
        }

        return true;
    }

    public int? GetLastBird(int branchIndex)
    {
        return _branches[branchIndex].LastOrDefault(b => b is not null);
    }

    public int GetEmptyCountOnBranch(int branchIndex)
    {
        return _branches[branchIndex].Reverse().TakeWhile(b => b is null).Count();
    }

    public int GetSameOnBranchFromEnd(int branchIndex, int? birdType)
    {
        return _branches[branchIndex].Reverse().SkipWhile(b => b is null).TakeWhile(b => b == birdType).Count();
    }

    public bool GameWon()
    {
        return _branches.All(b => b.All(bi => bi is null));
    }
}
