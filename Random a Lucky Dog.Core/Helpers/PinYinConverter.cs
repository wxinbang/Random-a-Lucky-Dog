﻿using Microsoft.International.Converters.PinYinConverter;
using RLD.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RLD.Core.Helpers
{
	public static class PinYinConverter
	{
		public static PinYin GetTotalPinYin(string str)
		{
			var chs = str.ToCharArray();
			//记录每个汉字的全拼
			Dictionary<int, List<string>> totalPinYins = new Dictionary<int, List<string>>();
			for (int i = 0; i < chs.Length; i++)
			{
				var pinyins = new List<string>();
				var ch = chs[i];
				//是否是有效的汉字
				if (ChineseChar.IsValidChar(ch))
				{
					ChineseChar cc = new ChineseChar(ch);
					pinyins = cc.Pinyins.Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
				}
				else
				{
					pinyins.Add(ch.ToString());
				}

				//去除声调，转小写
				pinyins = pinyins.ConvertAll(p => Regex.Replace(p, @"\d", "").ToLower());
				//去重
				pinyins = pinyins.Where(p => !string.IsNullOrWhiteSpace(p)).Distinct().ToList();
				if (pinyins.Any())
				{
					totalPinYins[i] = pinyins;
				}
			}
			PinYin result = new PinYin();
			foreach (var pinyins in totalPinYins)
			{
				var items = pinyins.Value;
				if (result.TotalPinYin.Count <= 0)
				{
					result.TotalPinYin = items;
					result.FirstPinYin = items.ConvertAll(p => p.Substring(0, 1)).Distinct().ToList();
				}
				else
				{
					//全拼循环匹配
					var newTotalPinYins = new List<string>();
					foreach (var totalPinYin in result.TotalPinYin)
					{
						newTotalPinYins.AddRange(items.Select(item => totalPinYin + item));
					}
					newTotalPinYins = newTotalPinYins.Distinct().ToList();
					result.TotalPinYin = newTotalPinYins;

					//首字母循环匹配
					var newFirstPinYins = new List<string>();
					foreach (var firstPinYin in result.FirstPinYin)
					{
						newFirstPinYins.AddRange(items.Select(item => firstPinYin + item.Substring(0, 1)));
					}
					newFirstPinYins = newFirstPinYins.Distinct().ToList();
					result.FirstPinYin = newFirstPinYins;
				}
			}
			return result;
		}
	}
}
