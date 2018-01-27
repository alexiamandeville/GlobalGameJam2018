using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This describes the action a user has to do to fulfil a rule's requirement.
class RuleSolution
{
	// Tool to use
	public ToolBox.Tool tool;

	// If the body type is needed, then set "isBodyPartSpecific"
	public bool isBodyPartSpecific = false;
	public BodyPartType bodyPart;

	// Helper constructor
	public RuleSolution( ToolBox.Tool t ) {
		tool = t;
	}

	public RuleSolution( ToolBox.Tool t, BodyPartType b ) {
		tool = t;
		isBodyPartSpecific = true;
		bodyPart = b;
	}
}

// Intentionally not a MonoBehavior: these are all static functions to
// help with applying game rules and seeing if the user did the right thing
// or the wrong thing!

enum RuleType
{
	ExactCount,			// Rule is applied if there is exactly X number of the given symptom across the body. Can be zero.
	MinCount,			// Rule is applied if there is exactly X or more number of the given symptom across the body.
	ExactMatch,			// Rule is applied if the matching pattern (color, body part, symptom) is matched.
};


// A rule is something that matches on given problem body part and symptom, or just the symptom
// A rule mutates during gameplay: for rules that require multiple actions, we remove a "RuleAction"
// as the user executes them. For example if the solution is to "Administer pills to the groin and
// put ointment on leg", then we first remove the first action, and if the user does the second we
// then remove the second action and consider the rule fulfilled.
class Rule
{
	// Based on a rule, we re-interpret the below params..
	public RuleType ruleType;

	// ExactCount, MinCount:
	public int count = 0;
	public Symptom countSymptom;

	// ExactMatch:
	public BodyPartType exactBodyPart;
	public Symptom exactSymptom;
	public BodyPartColor exactColor;

	// Rules can have one or more requirements to fulfill
	public List< RuleSolution > ruleSolutions = new List< RuleSolution >();

	// Helper constructor
	public Rule( RuleType type ) {
		ruleType = type;
	}
};

public class RulesSystem {

	static List< Rule > rules = null;

	// Initialize will fully re-author the rules, resetting the rule states!
	static void Initialize()
	{
		// Build our list of rules..
		rules = new List< Rule >();
		Rule rule = null;

		// No blood spurts -> Administer pill
		rule = new Rule( RuleType.ExactCount );
		rule.count = 0;
		rule.countSymptom = Symptom.BloodSpurts;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Pill ) );
		rules.Add (rule);

		// Blood from more than one area -> ointment on groin
		rule = new Rule( RuleType.MinCount );
		rule.count = 2;
		rule.countSymptom = Symptom.BloodSpurts;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Ointment, BodyPartType.Groin ) );
		rules.Add (rule);

		// Blood from head and green color -> Administer pills to the groin and put ointment on leg
		rule = new Rule( RuleType.ExactMatch );
		rule.exactBodyPart = BodyPartType.Head;
		rule.exactSymptom = Symptom.BloodSpurts;
		rule.exactColor = BodyPartColor.Green;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Pill, BodyPartType.Groin ) );
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Ointment, BodyPartType.LeftLeg ) );
		rules.Add (rule);
	}

}
