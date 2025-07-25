﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Service.DTOs.Requests.Payment
{
    public class PaymentDataDto
    {
        /// <summary>
        public int Amount { get; set; }

        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Danh sách sản phẩm/item để PayOs show chi tiết
        /// </summary>
        public List<PaymentItemDto> Items { get; set; } = new();

        public string CancelUrl { get; set; } = string.Empty;

        public string ReturnUrl { get; set; } = string.Empty;
    }
}
