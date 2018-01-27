﻿using System.Collections;
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
	public bool exactColorIsSpecific = true;
	public bool exactColorNegate = false; // Apply negation (if white, then any other color is accepted)
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
	public static void Initialize()
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

		// Remaining conditions w/Blood from head -> Tourniquet on leg
		// TODO: How do we implement an "else" rule? Meaning the above rules take precendence? Do we have rule groups?
		rule = new Rule( RuleType.ExactMatch );
		rule.exactBodyPart = BodyPartType.Head;
		rule.exactSymptom = Symptom.BloodSpurts;
		rule.exactColorIsSpecific = false;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Tourniquet, BodyPartType.LeftLeg ) );
		rules.Add (rule);

		// Blood from groin & skin is non-white -> Administer pill
		rule = new Rule( RuleType.ExactMatch );
		rule.exactBodyPart = BodyPartType.Groin;
		rule.exactSymptom = Symptom.BloodSpurts;
		rule.exactColorNegate = true;
		rule.exactColor = BodyPartColor.White;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Pill ) );
		rules.Add (rule);

		// Remaining conditions w/Blood from groin  -> Tourniquet on arm
		rule = new Rule( RuleType.ExactMatch );
		rule.exactBodyPart = BodyPartType.Groin;
		rule.exactSymptom = Symptom.BloodSpurts;
		rule.exactColorIsSpecific = false;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Tourniquet, BodyPartType.LeftArm ) );
		rules.Add (rule);

		// Blood from arm & skin has color -> Administer pill
		rule = new Rule( RuleType.ExactMatch );
		rule.exactBodyPart = BodyPartType.LeftArm;
		rule.exactSymptom = Symptom.BloodSpurts;
		rule.exactColorNegate = true;
		rule.exactColor = BodyPartColor.Normal;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Pill ) );
		rules.Add (rule);

		// Remaining conditions w/Blood from arm -> Tourniquet on groin
		rule = new Rule( RuleType.ExactMatch );
		rule.exactBodyPart = BodyPartType.LeftArm;
		rule.exactSymptom = Symptom.BloodSpurts;
		rule.exactColorIsSpecific = false;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Tourniquet, BodyPartType.Groin ) );
		rules.Add (rule);

		// Blood from leg and red color -> Tourniquet head and injection to head
		rule = new Rule( RuleType.ExactMatch );
		rule.exactBodyPart = BodyPartType.LeftLeg;
		rule.exactSymptom = Symptom.BloodSpurts;
		rule.exactColor = BodyPartColor.Red;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Tourniquet, BodyPartType.Head ) );
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Injector, BodyPartType.Head ) );
		rules.Add (rule);

		// Remaining conditions w/Blood from leg -> Tourniquet on head
		rule = new Rule( RuleType.ExactMatch );
		rule.exactBodyPart = BodyPartType.LeftLeg;
		rule.exactSymptom = Symptom.BloodSpurts;
		rule.exactColorIsSpecific = false;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Tourniquet, BodyPartType.Head ) );
		rules.Add (rule);
	}

	// Eval a rule: given the current body state, the tool applied and to what body part, we apply our rules
	// Returns true if we did something right (even if the rule isn't resolved) and false on a mistake
	public static bool EvaluateCure( BodyPart[] bodyParts, ToolBox.Tool tool, BodyPartType bodyPart )
	{
		// We have a TOOL, a BODY part, and a SYMPTOM on this body part..
		Symptom symptom = bodyParts[(int)bodyPart].symptom;

		// How many of each symptom do we have?
		int bloodSpurtCount = CountSymptom(bodyParts, Symptom.BloodSpurts);
		int painCount = CountSymptom(bodyParts, Symptom.Pain);
		int heartbeatCount = CountSymptom(bodyParts, Symptom.Heartbeat);
		int skinRashesCount = CountSymptom(bodyParts, Symptom.SkinRashes);

		// Todo..
		return false;
	}


	// Helper function: returns the number of body parts that have the given symptom
	static int CountSymptom( BodyPart[] bodyParts, Symptom symptom )
	{
		// I'm sure there is a lambda way of doing this
		int count = 0;

		int bodyPartCount = System.Enum.GetNames(typeof(BodyPartType)).Length;
		for (int i = 0; i < bodyPartCount; i++)
		{
			if (bodyParts[i].symptom == symptom)
				count++;
		}

		return count;
	}

}
