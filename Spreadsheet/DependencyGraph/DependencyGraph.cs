// Class that represents dependency graphs.
//
// @author Ethan Andrews
// @version September 7, 2023

using System.Runtime.CompilerServices;

namespace SpreadsheetUtilities;

/// <summary>
/// (s1,t1) is an ordered pair of strings
/// t1 depends on s1; s1 must be evaluated before t1
/// 
/// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
/// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
/// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
/// set, and the element is already in the set, the set remains unchanged.
/// 
/// Given a DependencyGraph DG:
/// 
///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
///        (The set of things that depend on s)    
///        
///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
///        (The set of things that s depends on) 
//
// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
//     dependents("a") = {"b", "c"}
//     dependents("b") = {"d"}
//     dependents("c") = {}
//     dependents("d") = {"d"}
//     dependees("a") = {}
//     dependees("b") = {"a"}
//     dependees("c") = {"a"}
//     dependees("d") = {"b", "d"}
/// </summary>
public class DependencyGraph
{
    private Dictionary<String, HashSet<String>> dependents;
    private Dictionary<String, HashSet<String>> dependees;
    private int size;

    /// <summary>
    /// Creates an empty DependencyGraph.
    /// </summary>
    public DependencyGraph()
    {
        dependents = new Dictionary<String, HashSet<String>>();
        dependees = new Dictionary<string, HashSet<string>>();
        size = 0;
    }


    /// <summary>
    /// The number of ordered pairs in the DependencyGraph.
    /// This is an example of a property.
    /// </summary>
    public int NumDependencies
    {
        get { return size; }
    }


    /// <summary>
    /// Returns the size of dependees(s),
    /// that is, the number of things that s depends on.
    /// </summary>
    public int NumDependees(string s)
    {
        // Checks if s is in dependees
        if (dependees.ContainsKey(s))
        {
            return dependees[s].Count;
        } 
        
        else
        {
            return 0;
        }
    }


    /// <summary>
    /// Reports whether dependents(s) is non-empty.
    /// </summary>
    public bool HasDependents(string s)
    {
        // Checks if dependents contains the key s
        if (dependents.ContainsKey(s))
        {
            // Returns whether or not it is empty
            return dependents[s].Count > 0;
        }

        else
        {
            return false;
        }
  
    }


    /// <summary>
    /// Reports whether dependees(s) is non-empty.
    /// </summary>
    public bool HasDependees(string s)
    {
        // Checks if dependees contains the key s
        if (dependees.ContainsKey(s))
        {
            // Returns whether or not it is empty
            return dependees[s].Count > 0;
        }

        else
        {
            return false;
        }
    }


    /// <summary>
    /// Enumerates dependents(s).
    /// </summary>
    public IEnumerable<string> GetDependents(string s)
    {
        if (dependents.ContainsKey(s))
        {
            // Returns hash set of dependents
            return dependents[s];
        }

        else
        {
            return new HashSet<String>();
        }

    }


    /// <summary>
    /// Enumerates dependees(s).
    /// </summary>
    public IEnumerable<string> GetDependees(string s)
    {
        if (dependees.ContainsKey(s))
        {
            // Returns hash set of dependees
            return dependees[s];
        }

        else
        {
            return new HashSet<String>();
        }
    }


    /// <summary>
    /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
    /// 
    /// <para>This should be thought of as:</para>   
    /// 
    ///   t depends on s
    ///
    /// </summary>
    /// <param name="s"> s must be evaluated first. T depends on S</param>
    /// <param name="t"> t cannot be evaluated until s is</param>
    public void AddDependency(string s, string t)
    {
        // Checks if s is in dictionary
        if (dependents.ContainsKey(s))
        {
            // If successfully added, add 1 to the size
            if (dependents[s].Add(t))
            {
                size++;
            }
        }

        // If dependents does not contain the key s, add new set with t to dictionary
        else
        {
            dependents.Add(s, new HashSet<string> { t } );
            size++;
        }


        // Checks if t is in dictionary
        if (dependees.ContainsKey(t))
        {
            dependees[t].Add(s);
        }

        // If dependents does not contain the key t, add new set with t to dictionary
        else
        {
            dependees.Add(t, new HashSet<string> { s });
        }
    }


    /// <summary>
    /// Removes the ordered pair (s,t), if it exists
    /// </summary>
    /// <param name="s"></param>
    /// <param name="t"></param>
    public void RemoveDependency(string s, string t)
    {
        // If the ordered pair does not exist in the dictionaries, return out of function.
        if (!dependents.ContainsKey(s) || !dependees.ContainsKey(t))
        {
            return;
        }

        if (dependents[s].Contains(t))
        {
            if (dependents[s].Remove(t))
            {
                // If successfully removed, remove 1 from the size;
                size--;
            }
        }

        if (dependees[t].Contains(s))
        {
            dependees[t].Remove(s);
        }
    }


    /// <summary>
    /// Removes all existing ordered pairs of the form (s,r).  Then, for each
    /// t in newDependents, adds the ordered pair (s,t).
    /// </summary>
    public void ReplaceDependents(string s, IEnumerable<string> newDependents)
    {
        IEnumerable<String> oldDependents = GetDependents(s);

        foreach (String oldDependent in oldDependents)
        {
            RemoveDependency(s, oldDependent);
        }

        foreach (String newDependent in newDependents)
        {
            AddDependency(s, newDependent);
        }
    }


    /// <summary>
    /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
    /// t in newDependees, adds the ordered pair (t,s).
    /// </summary>
    public void ReplaceDependees(string s, IEnumerable<string> newDependees)
    {
        IEnumerable<String> oldDependees = GetDependees(s);

        foreach (String oldDependee in oldDependees)
        {
            RemoveDependency(oldDependee, s);
        }

        foreach (String newDependee in newDependees)
        {
            AddDependency(newDependee, s);
        }
    }
}