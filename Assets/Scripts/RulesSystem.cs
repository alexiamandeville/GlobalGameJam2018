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
	ExactCount,			// Checks if there is X number of Y symptoms on fixesBodyPartType.
	MinCount,			// Checks if there is 1 number of Y sumptom on fixesBodyPartType and at least X (inclusive) number overall.
	ExactMatch,			// Checks if there is an exact full match of symptom, body part, and color.
	ColorMatch,			// Check if there is a color match. Simple.
	Heartbeat,			// Check if heartbeat is below or above.
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
	public BodyPartType fixesBodyPartType;

	// ExactCount, MinCount:
	public int count = 0;
	public Symptom countSymptom;

	// ExactMatch, ColorMatch:
	public Symptom exactSymptom;
	public bool exactColorIsSpecific = true;
	public bool exactColorNegate = false; // Apply negation (if white, then any other color is accepted)
	public BodyPartColor exactColor;

	// Heartbeat:
	public int heartbeatValue = 0;
	public bool heartbeatGreater = false;

	// Rules can have one or more requirements to fulfill
	public List< RuleSolution > ruleSolutions = new List< RuleSolution >();

	// ID for debugging
	public static int IDCounter = 0;
	public int ID = 0;

	// Helper constructor
	public Rule( RuleType type, BodyPartType fixedBodyPart ) {
		ruleType = type;
		fixesBodyPartType = fixedBodyPart;
		ID = IDCounter++;
	}

	// Color match doesn't need this stuff
	public Rule( RuleType type ) {
		ruleType = type;
		ID = IDCounter++;
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
		Rule.IDCounter = 0;

		// Rules come from: https://docs.google.com/document/d/1_UhgbOGdm4a65ZjJpK3v32FN13OfTvGpUeiiQGWP5Us/edit

		/*** 1. Blood Spurts ***/

		// Head:

		// If blood spurts from the head (and only here), then use the tourniquet on leg.
		rule = new Rule( RuleType.ExactCount, BodyPartType.Head );
		rule.count = 1;
		rule.countSymptom = Symptom.BloodSpurts;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Tourniquet, BodyPartType.Leg ) );
		rules.Add (rule);

		// If blood spurts from the head, and another area, then apply the ointment on groin.
		rule = new Rule( RuleType.MinCount, BodyPartType.Head );
		rule.count = 2;
		rule.countSymptom = Symptom.BloodSpurts;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Ointment, BodyPartType.Groin ) );
		rules.Add (rule);

		// If blood spurts from the head, and the skin is a green color, then go to section 4.
		// No rule needed: User executes section 4 rules

		// Groin:

		// If blood spurts from the groin, then use the tourniquet on arm.
		rule = new Rule( RuleType.ExactCount, BodyPartType.Groin );
		rule.count = 1;
		rule.countSymptom = Symptom.BloodSpurts;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Tourniquet, BodyPartType.Arm ) );
		rules.Add (rule);

		// If blood spurts from the groin, and another area, then apply the ointment on groin.
		rule = new Rule( RuleType.MinCount, BodyPartType.Groin );
		rule.count = 2;
		rule.countSymptom = Symptom.BloodSpurts;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Ointment, BodyPartType.Groin ) );
		rules.Add (rule);

		// If blood spurts from the groin, and the skin is a color, administer the pill.
		rule = new Rule( RuleType.ExactMatch, BodyPartType.Groin );
		rule.exactSymptom = Symptom.BloodSpurts;
		rule.exactColorNegate = true;
		rule.exactColor = BodyPartColor.White;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Pill ) );
		rules.Add (rule);

		// Arm:

		// If blood spurts from the arm, then use the tourniquet on groin.
		rule = new Rule( RuleType.ExactCount, BodyPartType.Arm );
		rule.count = 1;
		rule.countSymptom = Symptom.BloodSpurts;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Tourniquet, BodyPartType.Groin ) );
		rules.Add (rule);

		// If blood spurts from the arm, and another area, then apply the ointment on groin.
		rule = new Rule( RuleType.MinCount, BodyPartType.Arm );
		rule.count = 2;
		rule.countSymptom = Symptom.BloodSpurts;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Ointment, BodyPartType.Groin ) );
		rules.Add (rule);

		// If blood spurts from the arm, and the skin is a color, administer the pill.
		rule = new Rule( RuleType.ExactMatch, BodyPartType.Arm );
		rule.exactSymptom = Symptom.BloodSpurts;
		rule.exactColorNegate = true;
		rule.exactColor = BodyPartColor.White;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Pill ) );
		rules.Add (rule);

		// Leg:

		// If blood spurts from the leg, then use the tourniquet on head.
		rule = new Rule( RuleType.ExactCount, BodyPartType.Leg );
		rule.count = 1;
		rule.countSymptom = Symptom.BloodSpurts;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Tourniquet, BodyPartType.Head ) );
		rules.Add (rule);

		// If blood spurts from the leg, and another area, then apply the ointment on groin.
		rule = new Rule( RuleType.MinCount, BodyPartType.Leg );
		rule.count = 2;
		rule.countSymptom = Symptom.BloodSpurts;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Ointment, BodyPartType.Groin ) );
		rules.Add (rule);

		// If blood spurts from the leg, and the skin is a red color, then go to section 4.
		// No rule needed: User executes section 4 rules

		/*** 2. Pain ***/

		// Head:  Administer the pill.  Go to section 4.
		rule = new Rule( RuleType.MinCount, BodyPartType.Head );
		rule.count = 1;
		rule.countSymptom = Symptom.Pain;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Pill ) );
		rules.Add (rule);

		// Groin: Apply ointment to the leg. If heartbeat is above 80, then go to section 3. If not, then administer pill.
		rule = new Rule( RuleType.MinCount, BodyPartType.Groin );
		rule.count = 1;
		rule.countSymptom = Symptom.Pain;
		// TODO: Heartbeat
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Ointment, BodyPartType.Leg ) );
		rules.Add (rule);

		// Arm: Apply ointment to the arm. If heartbeat is below 80, then go to section 4. If not, go to section 3.
		rule = new Rule( RuleType.MinCount, BodyPartType.Arm );
		rule.count = 1;
		rule.countSymptom = Symptom.Pain;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Ointment, BodyPartType.Arm ) );
		rules.Add (rule);

		// Leg: Apply ointment to the head. If blood spurts from the arm, go to section 1. If not, go to section 1.
		rule = new Rule( RuleType.MinCount, BodyPartType.Leg );
		rule.count = 1;
		rule.countSymptom = Symptom.Pain;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Ointment, BodyPartType.Head ) );
		rules.Add (rule);

		/*** 3. Heartbeat ***/

		// If heartbeat is above 80, then inject into the arm.
		rule = new Rule( RuleType.Heartbeat );
		rule.heartbeatValue = 80;
		rule.heartbeatGreater = true;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Injector, BodyPartType.Arm ) );
		rules.Add (rule);

		/*** 4. Skin Color / Rashes ***/

		// Green: Pills - groin,  Ointment - leg
		rule = new Rule( RuleType.ColorMatch );
		rule.exactColor = BodyPartColor.Green;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Pill, BodyPartType.Groin ) );
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Ointment, BodyPartType.Leg ) );
		rules.Add (rule);

		// Red: Tourniquet - head,  Injection - head
		rule = new Rule( RuleType.ColorMatch );
		rule.exactColor = BodyPartColor.Red;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Tourniquet, BodyPartType.Head ) );
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Injector, BodyPartType.Head ) );
		rules.Add (rule);

		// Blue: Tourniquet - head,  Injection - groin
		rule = new Rule( RuleType.ColorMatch );
		rule.exactColor = BodyPartColor.Blue;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Tourniquet, BodyPartType.Head ) );
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Injector, BodyPartType.Groin ) );
		rules.Add (rule);

		// White: Ointment - leg
		rule = new Rule( RuleType.ColorMatch );
		rule.exactColor = BodyPartColor.White;
		rule.ruleSolutions.Add( new RuleSolution( ToolBox.Tool.Ointment, BodyPartType.Leg ) );
		rules.Add (rule);
	}

	// Eval a rule: given the current body state, the tool applied and to what body part, we apply our rules
	// Returns true if we did something right (even if the rule isn't resolved) and false on a mistake
	public static bool EvaluateCure( BodyPart[] bodyParts, ToolBox.Tool playersTool, BodyPartType playersTargetBodyPart, BodyPartColor bodyColor, int bodyHeartbeat )
	{
		// Count number of each symptom we have
		int kSymptomCount = System.Enum.GetNames(typeof(Symptom)).Length;
		int[] symptomCount = new int[ kSymptomCount ];

		// How many of each symptom do we have?
		for( int i = 1; i < kSymptomCount; i++ )
			symptomCount[ i ] = CountSymptom(bodyParts, (Symptom)i);
		
		// For each rule, see which is applicable!
		foreach (Rule rule in rules) {
			bool ruleDoesApply = false;

			// ExactCount: Checks if there is X number of Y symptoms on fixesBodyPartType.
			if (rule.ruleType == RuleType.ExactCount) {

				// Check count of this symptom, followed by if the symptom matches
				if (symptomCount [(int)rule.countSymptom] == rule.count && bodyParts [(int)rule.fixesBodyPartType].symptom == rule.countSymptom) {
					ruleDoesApply = true;
				}

			}

			// MinCount: Checks if there is 1 number of Y sumptom on fixesBodyPartType and at least X (inclusive) number overall.
			else if (rule.ruleType == RuleType.MinCount) {

				if (symptomCount [(int)rule.countSymptom] >= rule.count && bodyParts [(int)rule.fixesBodyPartType].symptom == rule.countSymptom) {
					ruleDoesApply = true;
				}

			}

			// ExactMatch: Rule is applied if the matching pattern (color, body part, symptom) i.
			else if (rule.ruleType == RuleType.ExactMatch) {

				// Does the target body part have this symptom?
				bool symptomMatches = (rule.exactSymptom == bodyParts [(int)rule.fixesBodyPartType].symptom);

				bool colorMatches = true;
				if (rule.exactColorIsSpecific)
					colorMatches = (rule.exactColor == bodyColor);
				else if (rule.exactColorNegate)
					colorMatches = (rule.exactColor != bodyColor);

				if (symptomMatches && colorMatches) {
					ruleDoesApply = true;
				}

			}

			// ColorMatch: Check if there is a color match. Simple.
			else if (rule.ruleType == RuleType.ColorMatch) {

				ruleDoesApply = ( bodyColor == rule.exactColor );

			}

			// Heartbeat: Check if heartbeat is below or above.
			else if (rule.ruleType == RuleType.Heartbeat) {

				if( rule.heartbeatGreater == true )
				{
					ruleDoesApply = ( bodyHeartbeat > rule.heartbeatValue );
				}
			}


			// If rule applies, see if the user used the right tool. On success, update body and return true
			if (ruleDoesApply && EvaluateRuleSolutions (rule, playersTool, playersTargetBodyPart)) {
				bodyParts [(int)rule.fixesBodyPartType].symptom = Symptom.None;
				return true;
			}

		}

		// No success..
		return false;
	}

	// For each rule solution, see if it applies to the given params. Removes the rule
	// solution on success, returning true. Else return false.
	private static bool EvaluateRuleSolutions( Rule rule, ToolBox.Tool playersTool, BodyPartType playersTargetBodyPart )
	{
		for (int i = 0; i < rule.ruleSolutions.Count; i++) {
			RuleSolution ruleSolution = rule.ruleSolutions[ i ];

			if (ruleSolution.tool == playersTool) {
				bool isValidBodyPart = ( ruleSolution.bodyPart == playersTargetBodyPart );
				if ( ( ruleSolution.isBodyPartSpecific == false ) ||
					 ( ruleSolution.isBodyPartSpecific && isValidBodyPart ) ) {
					rule.ruleSolutions.RemoveAt (i);
					return true;
				}
			}
		}

		// Never matched
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
