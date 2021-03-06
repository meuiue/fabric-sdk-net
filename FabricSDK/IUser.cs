/*
 *  Copyright 2016, 2017 DTCC, Fujitsu Australia Software Technology, IBM - All Rights Reserved.
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *   http://www.apache.org/licenses/LICENSE-2.0
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using System.Collections.Generic;

namespace Hyperledger.Fabric.SDK
{
    /**
     * User - Is the interface needed to be implemented by embedding application of the SDK
     */
    public interface IUser
    {

        /**
         * Get the name that identifies the user.
         *
         * @return the user name.
         */

        string Name { get; }

        /**
         * Get the roles to which the user belongs.
         *
         * @return role names.
         */
        HashSet<string> Roles { get; }

        /**
         * Get the user's account
         *
         * @return the account name
         */
        string Account { get; }

        /**
         * Get the user's affiliation.
         *
         * @return the affiliation.
         */
        string Affiliation { get; }

        /**
         * Get the user's enrollment certificate information.
         *
         * @return the enrollment information.
         */
        IEnrollment Enrollment { get; }

        /**
         * Get the Membership Service Provider Identifier provided by the user's organization.
         *
         * @return MSP Id.
         */
        string MspId { get; }

    }
}
