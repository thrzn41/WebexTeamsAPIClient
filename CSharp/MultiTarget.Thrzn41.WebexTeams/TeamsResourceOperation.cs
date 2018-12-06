/* 
 * MIT License
 * 
 * Copyright(c) 2018 thrzn41
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Thrzn41.WebexTeams
{


    /// <summary>
    /// Resouces of Webex Teams.
    /// </summary>
    public enum TeamsResource
    {
        /// <summary>
        /// Unknown resource.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Message resource.
        /// </summary>
        Message,

        /// <summary>
        /// Space resource.
        /// </summary>
        Space,

        /// <summary>
        /// Space membership resource.
        /// </summary>
        SpaceMembership,

        /// <summary>
        /// Team resource.
        /// </summary>
        Team,

        /// <summary>
        /// Team membership resource.
        /// </summary>
        TeamMembership,

        /// <summary>
        /// Person resource.
        /// </summary>
        Person,

        /// <summary>
        /// Webhook resource.
        /// </summary>
        Webhook,

        /// <summary>
        /// Event resource.
        /// </summary>
        Event,

        /// <summary>
        /// License resource.
        /// </summary>
        License,

        /// <summary>
        /// Organization resource.
        /// </summary>
        Organization,

        /// <summary>
        /// Role resource.
        /// </summary>
        Role,

        /// <summary>
        /// Resource group resource.
        /// </summary>
        ResourceGroup,

        /// <summary>
        /// Resource group membership resource.
        /// </summary>
        ResourceGroupMembership,

        /// <summary>
        /// Access token resource.
        /// </summary>
        AccessToken,

        /// <summary>
        /// Guest user resource.
        /// </summary>
        GuestUser,
    }


    /// <summary>
    /// Operations of Webex Teams.
    /// </summary>
    public enum TeamsOperation
    {
        /// <summary>
        /// Unknown operation.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Create operation.
        /// </summary>
        Create,

        /// <summary>
        /// Get operation.
        /// </summary>
        Get,

        /// <summary>
        /// List operation.
        /// </summary>
        List,

        /// <summary>
        /// Update operation.
        /// </summary>
        Update,

        /// <summary>
        /// Delete operation.
        /// </summary>
        Delete,

    }




    /// <summary>
    /// Resouce and Operation of Webex Teams.
    /// </summary>
    public class TeamsResourceOperation
    {

        /// <summary>
        /// Resouce of Webex Teams.
        /// </summary>
        public readonly TeamsResource Resource;

        /// <summary>
        /// Operation of Webex Teams.
        /// </summary>
        public readonly TeamsOperation Operation;


        /// <summary>
        /// Creates TeamsResourceOperation.
        /// </summary>
        /// <param name="resource">Resouce of Webex Teams.</param>
        /// <param name="operation">Operation of Webex Teams.</param>
        internal TeamsResourceOperation(TeamsResource resource, TeamsOperation operation)
        {
            this.Resource  = resource;
            this.Operation = operation;
        }



        /// <summary>
        /// Gets resource name.
        /// This method will be a little faster than <see cref="Enum.ToString()"/>
        /// </summary>
        /// <param name="resource">Resouce of Webex Teams.</param>
        /// <returns>Name of the resource.</returns>
        public static string GetResourceName(TeamsResource resource)
        {
            string name;

            // This will be a tiny little bit faster than refrection base resource.ToString().
            switch (resource)
            {
                case TeamsResource.Unknown:
                    name = "Unknown";
                    break;

                case TeamsResource.Message:
                    name = "Message";
                    break;

                case TeamsResource.Space:
                    name = "Space";
                    break;

                case TeamsResource.SpaceMembership:
                    name = "SpaceMembership";
                    break;

                case TeamsResource.Team:
                    name = "Team";
                    break;

                case TeamsResource.TeamMembership:
                    name = "TeamMembership";
                    break;

                case TeamsResource.Person:
                    name = "Person";
                    break;

                case TeamsResource.Webhook:
                    name = "Webhook";
                    break;

                case TeamsResource.Event:
                    name = "Event";
                    break;

                case TeamsResource.License:
                    name = "License";
                    break;

                case TeamsResource.Organization:
                    name = "Organization";
                    break;

                case TeamsResource.Role:
                    name = "Role";
                    break;

                case TeamsResource.ResourceGroup:
                    name = "ResourceGroup";
                    break;

                case TeamsResource.ResourceGroupMembership:
                    name = "ResourceGroupMembership";
                    break;

                case TeamsResource.AccessToken:
                    name = "AccessToken";
                    break;

                case TeamsResource.GuestUser:
                    name = "GuestUser";
                    break;

                default:
                    name = resource.ToString();
                    break;
            }

            return name;
        }


        /// <summary>
        /// Gets operation name.
        /// This method will be a little faster than <see cref="Enum.ToString()"/>
        /// </summary>
        /// <param name="operation">Operation of Webex Teams.</param>
        /// <returns>Name of the operation.</returns>
        public static string GetOperationName(TeamsOperation operation)
        {
            string name;

            // This will be a tiny little bit faster than refrection base operation.ToString().
            switch (operation)
            {
                case TeamsOperation.Unknown:
                    name = "Unknown";
                    break;

                case TeamsOperation.Create:
                    name = "Create";
                    break;

                case TeamsOperation.Get:
                    name = "Get";
                    break;

                case TeamsOperation.List:
                    name = "List";
                    break;

                case TeamsOperation.Update:
                    name = "Update";
                    break;

                case TeamsOperation.Delete:
                    name = "Delete";
                    break;

                default:
                    name = operation.ToString();
                    break;
            }

            return name;
        }


        /// <summary>
        /// Gets resource name.
        /// </summary>
        /// <returns>Name of the resource.</returns>
        public string GetResourceName()
        {
            return GetResourceName(this.Resource);
        }

        /// <summary>
        /// Gets operation name.
        /// </summary>
        /// <returns>Name of the operation.</returns>
        public string GetOperationName()
        {
            return GetOperationName(this.Operation);
        }




        /// <summary>
        /// Determines whether this instance and another specified <see cref="TeamsResourceOperation"/> object have the same value.
        /// </summary>
        /// <param name="value">The <see cref="TeamsResourceOperation"/> to compare to this instance.</param>
        /// <returns>true if the value of the parameter is the same as the value of this instance; otherwise, false. If value is null, the method returns false.</returns>
        public bool Equals(TeamsResourceOperation value)
        {
            if (Object.ReferenceEquals(value, null))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, value))
            {
                return true;
            }

            return (this.Resource == value.Resource && this.Operation == value.Operation);
        }


        /// <summary>
        /// Determines whether this instance and a specified object, which must also be a <see cref="TeamsResourceOperation"/> object, have the same value.
        /// </summary>
        /// <param name="obj">The event type to compare to this instance.</param>
        /// <returns>true if obj is a <see cref="TeamsResourceOperation"/> and its value is the same as this instance; otherwise, false. If obj is null, the method returns false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as TeamsResourceOperation);
        }


        /// <summary>
        /// Returns the hash code for this event type.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return (ToString()).GetHashCode();
        }


        /// <summary>
        /// Determines whether two specified <see cref="TeamsResourceOperation"/> have the same value.
        /// </summary>
        /// <param name="lhs">Left hand side value.</param>
        /// <param name="rhs">Right hand side value.</param>
        /// <returns>true if the two values have the same value.</returns>
        public static bool operator ==(TeamsResourceOperation lhs, TeamsResourceOperation rhs)
        {
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                {
                    return true;
                }

                return false;
            }

            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Determines whether two specified <see cref="TeamsResourceOperation"/> have the different value.
        /// </summary>
        /// <param name="lhs">Left hand side value.</param>
        /// <param name="rhs">Right hand side value.</param>
        /// <returns>true if the two values have the different value.</returns>
        public static bool operator !=(TeamsResourceOperation lhs, TeamsResourceOperation rhs)
        {
            return !(lhs == rhs);
        }


        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation of the value of this instance.</returns>
        public override string ToString()
        {
            return String.Format("{0}{1}", GetOperationName(), GetResourceName());
        }

    }

}
