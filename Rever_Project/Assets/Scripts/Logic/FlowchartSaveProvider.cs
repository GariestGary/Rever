using Fungus;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[RequireComponent(typeof(Flowchart))]
public class FlowchartSaveProvider : Saveable
{
    private Flowchart mainChart;

	private Dictionary<string, object> mainFlags = new Dictionary<string, object>();

	public override void Rise()
	{
		mainChart = GetComponent<Flowchart>();
	}

	public override object CaptureState()
	{
		string[] names = mainChart.GetVariableNames();

		for (int i = 0; i < names.Length; i++)
		{
			Variable v = mainChart.GetVariable(names[i]);

			if (mainFlags.ContainsKey(names[i]))
			{
				mainFlags[names[i]] = CastVariable(v);
			}
			else
			{
				mainFlags.Add(names[i], CastVariable(v));
			}
		}

		return mainFlags;
	}

	private object CastVariable(Variable variable)
	{
		if(variable.GetType() == typeof(IntegerVariable))
		{
			return (variable as IntegerVariable).Value;
		}

		if (variable.GetType() == typeof(FloatVariable))
		{
			return (variable as FloatVariable).Value;
		}

		if (variable.GetType() == typeof(StringVariable))
		{
			return (variable as StringVariable).Value;
		}

		if(variable.GetType() == typeof(BooleanVariable))
		{
			return (variable as BooleanVariable).Value;
		}

		return 0;
	}

	public override void RestoreState(string state)
	{
		mainFlags = JsonConvert.DeserializeObject<Dictionary<string, object>>(state);

		for (int i = 0; i < mainChart.VariableCount; i++)
		{
			string key = mainChart.Variables[i].Key;

			if (mainFlags.ContainsKey(key))
			{
				object val = mainFlags[key];

				if (val.GetType() == typeof(string))
				{
					mainChart.SetStringVariable(key, val as string);
					continue;
				}

				if (val.GetType() == typeof(Int64))
				{
					mainChart.SetIntegerVariable(key, Convert.ToInt32(val));
					continue;
				}

				if (val.GetType() == typeof(double))
				{
					mainChart.SetFloatVariable(key, Convert.ToSingle(val));
					continue;
				}

				if(val.GetType() == typeof(bool))
				{
					mainChart.SetBooleanVariable(key, (bool)val);
					continue;
				}

				Debug.LogError(val.GetType() + " type not implemented in flowchart save provider");
			}
			else
			{
				Debug.LogWarning("Save data do not contain corresponding data for " + key + " variable");
			}
		}
	}
}
