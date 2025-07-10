using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static ERS.ERSConsts;

namespace ERS;

public static class ERSConsts
{
    public const string Company_All = "All";
    public const string DbTablePrefix = "App";
    public const string DbSchema = null;

    public const string Value_F = "F";
    public const string Value_Y = "Y";
    public const string Value_N = "N";

    public enum RegionEnum
    {
        CN,
    }

    public enum CurrencyEnum
    {
        RMB,
    }

    public enum FormCode
    {
        /// <summary>
        /// 一般费用報銷
        /// </summary>
        CASH_1,
        /// <summary>
        /// 交際費報銷
        /// </summary>
        CASH_2,
        /// <summary>
        /// 预支金
        /// </summary>
        CASH_3,
        /// <summary>
        /// 批量报销
        /// </summary>
        CASH_4,
        /// <summary>
        /// 大量報銷
        /// </summary>
        CASH_5,
        /// <summary>
        /// 返台會議申請
        /// </summary>
        CASH_6,
    }

    /// <summary>
    /// 發票來源
    /// </summary>
    public enum InvoiceSourceEnum
    {
        //Description是对应的value
        [Description("invoice pool")]
        InvoicePool, //不能修改發票資料
        [Description("ocr")]
        OCR, //可以修改發票資料，改了之後就變成manual
        [Description("manual")]
        Manual
    }

    public enum InvoicePayTypeEnum
    {
        /// <summary>
        /// 已請款
        /// </summary>
        [Description("requested")]
        Requested,
        /// <summary>
        /// 待請款
        /// </summary>
        [Description("unrequested")]
        Unrequested,
        /// <summary>
        /// 已入賬
        /// </summary>
        [Description("recorded")]
        Recorded 
    }

    /// <summary>
    /// 数据字典
    /// </summary>
    public enum DataDictionaryEnum
    {
        [Description("responsible_party")]
        ResponsibleParty,
        [Description("tax_type")]
        TaxType,
        [Description("senario_category")]
        SenarioCategory,
        [Description("senario_calmethod")]
        CalMethod,
        [Description("senario_extraformcode")]
        ExtraFormCode,
    }

    public enum SenarioCategoryEnum
    {
        /// <summary>
        /// 報銷
        /// </summary>
        [Description("reimbursement")]
        Reimbursement,
        /// <summary>
        /// 預支
        /// </summary>
        [Description("advance")]
        Advance,
        /// <summary>
        /// 薪資請款
        /// </summary>
        [Description("payroll")]
        Payroll,
        /// <summary>
        /// 大量報銷
        /// </summary>
        [Description("mass")]
        MassReimbersement,
    }

    /// <summary>
    /// SAP过账码
    /// </summary>
    public enum SAPPostingKeyEnum
    {
        /// <summary>
        /// 借方
        /// </summary>
        [Description("40")]
        Debit,
        /// <summary>
        /// 贷方
        /// </summary>
        [Description("31")]
        Credit, 
    }

    /// <summary>
    /// SAP系统中过账的参考代码Document Type，凭证类型
    /// </summary>
    public enum SAPDocumentTypeEnum
    {
        /// <summary>
        /// 借方:費用
        /// </summary>
        KR,
    }
    public enum CashCarryDetailStatusEnum
    {
        /// <summary>
        /// 可编辑
        /// </summary>
        Y,
        /// <summary>
        /// 不可编辑
        /// </summary>
        N,
    }
    

}

public static class InvoiceSourceEnumExtensions
{
    /// <summary>
    /// 转换为对应的字符串值
    /// </summary>
    public static string ToValue(this InvoiceSourceEnum value)
    {
        // 通过反射获取枚举字段
        var field = value.GetType().GetField(value.ToString());

        // 获取 DescriptionAttribute 特性
        var attribute = (DescriptionAttribute)field
            .GetCustomAttributes(typeof(DescriptionAttribute), false)
            .FirstOrDefault();

        // 返回特性描述或抛出异常
        return attribute?.Description ?? throw new ArgumentOutOfRangeException();
    }
}

public static class InvoicePayTypeEnumExtensions
{
    /// <summary>
    /// 转换为对应的字符串值
    /// </summary>
    public static string ToValue(this InvoicePayTypeEnum value)
    {
        // 通过反射获取枚举字段
        var field = value.GetType().GetField(value.ToString());

        // 获取 DescriptionAttribute 特性
        var attribute = (DescriptionAttribute)field
            .GetCustomAttributes(typeof(DescriptionAttribute), false)
            .FirstOrDefault();

        // 返回特性描述或抛出异常
        return attribute?.Description ?? throw new ArgumentOutOfRangeException();
    }
}

public static class DataDictionaryEnumExtensions
{
    /// <summary>
    /// 转换为对应的字符串值
    /// </summary>
    public static string ToValue(this DataDictionaryEnum value)
    {
        // 通过反射获取枚举字段
        var field = value.GetType().GetField(value.ToString());

        // 获取 DescriptionAttribute 特性
        var attribute = (DescriptionAttribute)field
            .GetCustomAttributes(typeof(DescriptionAttribute), false)
            .FirstOrDefault();

        // 返回特性描述或抛出异常
        return attribute?.Description ?? throw new ArgumentOutOfRangeException();
    }
}

public static class SenarioCategoryEnumExtensions
{
    /// <summary>
    /// 转换为对应的字符串值
    /// </summary>
    public static string ToValue(this SenarioCategoryEnum value)
    {
        // 通过反射获取枚举字段
        var field = value.GetType().GetField(value.ToString());

        // 获取 DescriptionAttribute 特性
        var attribute = (DescriptionAttribute)field
            .GetCustomAttributes(typeof(DescriptionAttribute), false)
            .FirstOrDefault();

        // 返回特性描述或抛出异常
        return attribute?.Description ?? throw new ArgumentOutOfRangeException();
    }
}

public static class SAPPostingKeyEnumExtensions
{
    /// <summary>
    /// 转换为对应的字符串值
    /// </summary>
    public static string ToValue(this SAPPostingKeyEnum value)
    {
        // 通过反射获取枚举字段
        var field = value.GetType().GetField(value.ToString());

        // 获取 DescriptionAttribute 特性
        var attribute = (DescriptionAttribute)field
            .GetCustomAttributes(typeof(DescriptionAttribute), false)
            .FirstOrDefault();

        // 返回特性描述或抛出异常
        return attribute?.Description ?? throw new ArgumentOutOfRangeException();
    }
}
