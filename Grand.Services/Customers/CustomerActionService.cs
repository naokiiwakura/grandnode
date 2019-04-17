﻿using Grand.Core;
using Grand.Core.Caching;
using Grand.Core.Data;
using Grand.Core.Domain.Customers;
using Grand.Services.Events;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grand.Services.Customers
{
    public partial class CustomerActionService: ICustomerActionService
    {
        #region Fields
        private const string CUSTOMER_ACTION_TYPE = "Grand.customer.action.type";
        private readonly IRepository<CustomerAction> _customerActionRepository;
        private readonly IRepository<CustomerActionType> _customerActionTypeRepository;
        private readonly IRepository<CustomerActionHistory> _customerActionHistoryRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public CustomerActionService(IRepository<CustomerAction> customerActionRepository,
            IRepository<CustomerActionType> customerActionTypeRepository,
            IRepository<CustomerActionHistory> customerActionHistoryRepository,
            IEventPublisher eventPublisher,
            ICacheManager cacheManager)
        {
            this._customerActionRepository = customerActionRepository;
            this._customerActionTypeRepository = customerActionTypeRepository;
            this._customerActionHistoryRepository = customerActionHistoryRepository;
            this._eventPublisher = eventPublisher;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Gets customer action
        /// </summary>
        /// <param name="id">Customer action identifier</param>
        /// <returns>Customer Action</returns>
        public virtual Task<CustomerAction> GetCustomerActionById(string id)
        {
            return _customerActionRepository.GetByIdAsync(id);
        }


        /// <summary>
        /// Gets all customer actions
        /// </summary>
        /// <returns>Customer actions</returns>
        public virtual async Task<IList<CustomerAction>> GetCustomerActions()
        {
            var query = _customerActionRepository.Table;
            return await query.ToListAsync();
        }

        /// <summary>
        /// Inserts a customer action
        /// </summary>
        /// <param name="CustomerAction">Customer action</param>
        public virtual async Task InsertCustomerAction(CustomerAction customerAction)
        {
            if (customerAction == null)
                throw new ArgumentNullException("customerAction");

            await _customerActionRepository.InsertAsync(customerAction);

            //event notification
            await _eventPublisher.EntityInserted(customerAction);

        }

        /// <summary>
        /// Delete a customer action
        /// </summary>
        /// <param name="customerAction">Customer action</param>
        public virtual async Task DeleteCustomerAction(CustomerAction customerAction)
        {
            if (customerAction == null)
                throw new ArgumentNullException("customerAction"); 

            await _customerActionRepository.DeleteAsync(customerAction);

            //event notification
            await _eventPublisher.EntityDeleted(customerAction);

        }

        /// <summary>
        /// Updates the customer action
        /// </summary>
        /// <param name="customerTag">Customer tag</param>
        public virtual async Task UpdateCustomerAction(CustomerAction customerAction)
        {
            if (customerAction == null)
                throw new ArgumentNullException("customerAction");

            await _customerActionRepository.UpdateAsync(customerAction);

            //event notification
            await _eventPublisher.EntityUpdated(customerAction);
        }

        #endregion

        #region Condition Type

        public virtual async Task<IList<CustomerActionType>> GetCustomerActionType()
        {
            var query = _customerActionTypeRepository.Table;
            return await query.ToListAsync();
        }

        public virtual async Task<IPagedList<CustomerActionHistory>> GetAllCustomerActionHistory(string customerActionId, int pageIndex = 0, int pageSize = 2147483647)
        {
            var query = from h in _customerActionHistoryRepository.Table
                        where h.CustomerActionId == customerActionId
                        select h;
            return await PagedList<CustomerActionHistory>.Create(query, pageIndex, pageSize);
        }

        public virtual async Task<CustomerActionType> GetCustomerActionTypeById(string id)
        {
            return await _customerActionTypeRepository.GetByIdAsync(id);
        }

        public virtual async Task UpdateCustomerActionType(CustomerActionType customerActionType)
        {
            if (customerActionType == null)
                throw new ArgumentNullException("customerActionType");

            await _customerActionTypeRepository.UpdateAsync(customerActionType);

            //clear cache
            _cacheManager.Remove(CUSTOMER_ACTION_TYPE);
            //event notification
            await _eventPublisher.EntityUpdated(customerActionType);
        }

        #endregion

    }
}
