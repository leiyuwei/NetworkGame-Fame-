using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BattleFramework.Data;
using CSV;
using System.IO;

namespace MMO
{
	public class CSVManager : SingleMonoBehaviour<CSVManager>
	{
		//	public const string CSV_PATH = @"Assets/CSV";//
		private const string CSV_UNIT = "m_unit";
		private CsvContext mCsvContext;
		List<MUnit> mUnitList;
		Dictionary<int,MUnit> mUnitDic;

		protected override void Awake ()
		{
			base.Awake ();
			StartLoading ();
		}

		byte[] GetCSV (string fileName)
		{
			//#if UNITY_EDITOR
			//TODO 因为时间关系暂时用Resources，放到固定的文件夹下面，可以编辑最佳。
			return Resources.Load<TextAsset> ("CSV/" + fileName).bytes;
			//#else
			//return ResourcesManager.Ins.GetCSV (fileName);
			//#endif
		}

		void StartLoading ()
		{
			mCsvContext = new CsvContext ();
			LoadUnitTable ();
		}

		public MUnit GetUnit (int unitId)
		{
			if (mUnitDic.ContainsKey (unitId))
				return mUnitDic [unitId];
			else {
				return mUnitList [0];
			}
		}

		void LoadUnitTable ()
		{
			mUnitList = CreateCSVList<MUnit> (CSV_UNIT);
			mUnitDic = GetDictionary<MUnit> (mUnitList);
		}

		List<T> CreateCSVList<T> (string csvname) where T:BaseCSVStructure, new()
		{
			var stream = new MemoryStream (GetCSV (csvname));
			var reader = new StreamReader (stream);
			IEnumerable<T> list = mCsvContext.Read<T> (reader);
			return new List<T> (list);
		}

		Dictionary<int,T> GetDictionary<T> (List<T> list) where T : BaseCSVStructure
		{
			Dictionary<int,T> dic = new Dictionary<int, T> ();
			foreach (T t in list) {
				if (!dic.ContainsKey (t.id))
					dic.Add (t.id, t);
			}
			return dic;
		}

	}
}
